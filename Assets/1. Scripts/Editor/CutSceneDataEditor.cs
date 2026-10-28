using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CutSceneData))]
public class CutSceneDataEditor : Editor
{
    private const string StylePath = "Assets/1. Scripts/Editor/CutSceneDataEditor.uss";
    private static readonly List<string> TypeNames = new List<string> { "대사", "함수 실행", "대기" };
    private static readonly List<string> WaitModes = new List<string> { "화면 유지", "화면 숨기기" };
    private readonly List<Card> cards = new List<Card>();
    private readonly List<bool> expanded = new List<bool>();
    private readonly List<VisualElement> insertions = new List<VisualElement>();
    private VisualElement root;
    private ScrollView list;
    private SerializedProperty actions;
    private Image leftImage;
    private Image rightImage;
    private Label previewName;
    private Label previewText;
    private Label previewStatus;
    private Label countLabel;
    private Button previous;
    private Button next;
    private string search = "";
    private int selected;
    private int dropIndex = -1;

    private sealed class Card
    {
        public VisualElement Root;
        public Foldout Foldout;
        public Label Summary;
        public Label Options;
        public HelpBox Warning;
        public DropdownField Type;
        public ObjectField Sprite;
        public VisualElement DialogueFields;
        public VisualElement FunctionFields;
        public TextField Method;
        public DropdownField WaitMode;
        public HelpBox WaitHelp;
    }

    private void OnEnable()
    {
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;
        root?.Unbind();
        root = null;
    }

    public override VisualElement CreateInspectorGUI()
    {
        serializedObject.Update();
        actions = serializedObject.FindProperty("cutSceneActions");
        root = new VisualElement();
        root.AddToClassList("cutscene-editor");
        var sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath);
        if (sheet != null) root.styleSheets.Add(sheet);
        if (!EditorGUIUtility.isProSkin) root.AddToClassList("light");

        var title = new Label("CUTSCENE / 컷씬 편집");
        title.AddToClassList("title");
        root.Add(title);
        var sceneName = new TextField("컷씬 이름");
        sceneName.BindProperty(serializedObject.FindProperty("cutSceneName"));
        root.Add(sceneName);
        BuildPreview();

        var toolbar = new VisualElement();
        toolbar.AddToClassList("toolbar");
        var searchField = new ToolbarSearchField { tooltip = "닉네임, 대사, 함수 대상으로 검색" };
        searchField.AddToClassList("search");
        searchField.RegisterValueChangedCallback(evt => { search = evt.newValue; FilterCards(); });
        toolbar.Add(searchField);
        toolbar.Add(new Button(() => SetExpanded(true)) { text = "펼치기" });
        toolbar.Add(new Button(() => SetExpanded(false)) { text = "접기" });
        root.Add(toolbar);
        countLabel = new Label();
        countLabel.AddToClassList("muted");
        root.Add(countLabel);
        list = new ScrollView(ScrollViewMode.Vertical);
        list.AddToClassList("action-list");
        root.Add(list);
        Rebuild();
        root.TrackSerializedObjectValue(serializedObject, _ =>
        {
            if (cards.Count != actions.arraySize) Rebuild();
            else Refresh();
        });
        return root;
    }

    private void BuildPreview()
    {
        var preview = new Foldout { text = "미리보기", value = true, viewDataKey = "cutscene-preview" };
        preview.AddToClassList("preview");
        var navigation = new VisualElement();
        navigation.AddToClassList("toolbar");
        previous = new Button(() => Select(selected - 1, true)) { text = "◀ 이전" };
        next = new Button(() => Select(selected + 1, true)) { text = "다음 ▶" };
        previewStatus = new Label();
        previewStatus.AddToClassList("preview-status");
        navigation.Add(previous);
        navigation.Add(previewStatus);
        navigation.Add(next);
        preview.Add(navigation);
        var portraits = new VisualElement();
        portraits.AddToClassList("portraits");
        leftImage = new Image { scaleMode = ScaleMode.ScaleToFit };
        rightImage = new Image { scaleMode = ScaleMode.ScaleToFit };
        leftImage.AddToClassList("portrait");
        rightImage.AddToClassList("portrait");
        portraits.Add(leftImage);
        portraits.Add(rightImage);
        preview.Add(portraits);
        previewName = new Label();
        previewName.AddToClassList("preview-name");
        previewText = new Label { enableRichText = false };
        previewText.AddToClassList("preview-text");
        preview.Add(previewName);
        var dialogue = new ScrollView(ScrollViewMode.Vertical);
        dialogue.AddToClassList("preview-dialogue");
        dialogue.Add(previewText);
        preview.Add(dialogue);
        root.Add(preview);
    }

    private void Rebuild()
    {
        list.Unbind();
        list.Clear();
        cards.Clear();
        insertions.Clear();
        while (expanded.Count < actions.arraySize) expanded.Add(true);
        if (expanded.Count > actions.arraySize) expanded.RemoveRange(actions.arraySize, expanded.Count - actions.arraySize);
        selected = Mathf.Clamp(selected, 0, Mathf.Max(0, actions.arraySize - 1));
        AddInsertion(0);
        for (int i = 0; i < actions.arraySize; i++)
        {
            BuildCard(i);
            AddInsertion(i + 1);
        }
        Refresh();
    }

    private void AddInsertion(int index)
    {
        var button = new Button(() => ShowInsertMenu(index)) { text = "+", tooltip = "이 위치에 액션 삽입" };
        button.AddToClassList("insertion");
        list.Add(button);
        insertions.Add(button);
    }

    private void BuildCard(int index)
    {
        var property = actions.GetArrayElementAtIndex(index);
        var card = new Card { Root = new VisualElement() };
        card.Root.AddToClassList("action-card");
        card.Root.RegisterCallback<PointerDownEvent>(_ => Select(index, false), TrickleDown.TrickleDown);
        card.Type = new DropdownField("Type", TypeNames, (int)((CutSceneData)target).cutSceneActions[index].Type);
        card.Type.RegisterValueChangedCallback(evt => SetType(index, (CutSceneActionType)TypeNames.IndexOf(evt.newValue)));
        card.Root.Add(card.Type);
        var header = new VisualElement();
        header.AddToClassList("toolbar");
        var handle = new Label("≡") { tooltip = "드래그하여 순서 변경 (검색 중에는 비활성)" };
        handle.AddToClassList("drag-handle");
        handle.AddManipulator(new ActionDrag(this, index));
        header.Add(handle);
        header.Add(new Label($"{index + 1:00}") { name = "number" });
        card.Summary = new Label { enableRichText = false };
        card.Summary.AddToClassList("summary");
        header.Add(card.Summary);
        header.Add(new Button(() => Insert(index + 1, index, false)) { text = "복제" });
        header.Add(new Button(() => Delete(index)) { text = "삭제" });
        card.Root.Add(header);
        card.Options = new Label();
        card.Options.AddToClassList("options");
        card.Root.Add(card.Options);
        card.Foldout = new Foldout { text = "설정", value = expanded[index] };
        card.Foldout.RegisterValueChangedCallback(evt =>
        {
            if (evt.target == card.Foldout) expanded[index] = evt.newValue;
        });
        card.DialogueFields = new VisualElement();
        card.DialogueFields.Add(BindText(property, "name", "닉네임", "빈 값은 닉네임 표시를 비웁니다."));
        var text = BindText(property, "text", "대사");
        text.multiline = true;
        text.verticalScrollerVisibility = ScrollerVisibility.Auto;
        text.AddToClassList("dialogue-input");
        card.DialogueFields.Add(text);
        card.Sprite = new ObjectField("이미지") { objectType = typeof(Sprite), allowSceneObjects = false, tooltip = "None이면 이미지를 지웁니다." };
        card.Sprite.SetValueWithoutNotify(((CutSceneData)target).cutSceneActions[index].DisplaySprite);
        card.Sprite.RegisterValueChangedCallback(evt => SetSprite(index, (Sprite)evt.newValue));
        card.DialogueFields.Add(card.Sprite);
        var side = new Toggle("왼쪽에 표시");
        side.BindProperty(property.FindPropertyRelative("isLeft"));
        card.DialogueFields.Add(side);
        card.Foldout.Add(card.DialogueFields);
        card.FunctionFields = new VisualElement();
        card.FunctionFields.Add(BindText(property, "clazz", "대상 GameObject", "현재 씬에서 활성 GameObject를 이름으로 찾습니다."));
        card.Method = BindText(property, "method", "메서드 이름");
        card.Method.isDelayed = true;
        card.FunctionFields.Add(card.Method);
        card.WaitMode = new DropdownField("대기 중 화면", WaitModes, 0);
        card.WaitMode.RegisterValueChangedCallback(evt => SetWaitMethod(index,
            evt.newValue == WaitModes[1] ? nameof(CutsceneManager.WaitUntilAndHide) : nameof(CutsceneManager.WaitUntil)));
        card.FunctionFields.Add(card.WaitMode);
        card.WaitHelp = new HelpBox("대상에는 CutsceneManager가 붙은 활성 GameObject 이름을 입력합니다. Continue() 호출 시 재개하며, 대기 중 게임 시간은 다시 흐릅니다.", HelpBoxMessageType.Info);
        card.FunctionFields.Add(card.WaitHelp);
        card.Foldout.Add(card.FunctionFields);
        card.Warning = new HelpBox("", HelpBoxMessageType.Warning);
        card.Foldout.Add(card.Warning);
        card.Root.Add(card.Foldout);
        list.Add(card.Root);
        cards.Add(card);
    }

    private static TextField BindText(SerializedProperty parent, string field, string label, string tooltip = null)
    {
        var input = new TextField(label) { tooltip = tooltip };
        input.BindProperty(parent.FindPropertyRelative(field));
        return input;
    }

    private void ShowInsertMenu(int index)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("빈 액션"), false, () => Insert(index, -1, false));
        if (index > 0)
        {
            menu.AddItem(new GUIContent("이전 화자 유지"), false, () => Insert(index, index - 1, true));
            menu.AddItem(new GUIContent("이전 액션 복제"), false, () => Insert(index, index - 1, false));
        }
        menu.ShowAsContext();
    }

    private void Insert(int index, int sourceIndex, bool speakerOnly)
    {
        serializedObject.Update();
        // Unity duplicates array elements on insertion; explicitly initialize every serialized field.
        var source = sourceIndex >= 0 ? ((CutSceneData)target).cutSceneActions[sourceIndex] : null;
        var copy = new CutSceneAction
        {
            name = source?.name ?? "",
            text = source != null && !speakerOnly ? source.text : "",
            sprite = source?.sprite,
            isLeft = source != null && source.isLeft,
            clearSprite = source != null && !speakerOnly && source.clearSprite,
            callFunction = source != null && !speakerOnly && source.callFunction,
            clazz = source != null && !speakerOnly ? source.clazz : "",
            method = source != null && !speakerOnly ? source.method : ""
        };
        if (speakerOnly)
        {
            EvaluatePreview(sourceIndex, out copy.name, out copy.sprite, out copy.isLeft, out _);
        }
        actions.InsertArrayElementAtIndex(index);
        var added = actions.GetArrayElementAtIndex(index);
        added.FindPropertyRelative("name").stringValue = copy.name;
        added.FindPropertyRelative("text").stringValue = copy.text;
        added.FindPropertyRelative("sprite").objectReferenceValue = copy.sprite;
        added.FindPropertyRelative("isLeft").boolValue = copy.isLeft;
        added.FindPropertyRelative("clearSprite").boolValue = copy.clearSprite;
        added.FindPropertyRelative("callFunction").boolValue = copy.callFunction;
        added.FindPropertyRelative("clazz").stringValue = copy.clazz;
        added.FindPropertyRelative("method").stringValue = copy.method;
        expanded.Insert(index, true);
        selected = index;
        ApplyStructure("컷씬 액션 삽입");
    }

    private void Delete(int index)
    {
        serializedObject.Update();
        actions.DeleteArrayElementAtIndex(index);
        expanded.RemoveAt(index);
        if (selected > index) selected--;
        ApplyStructure("컷씬 액션 삭제");
    }

    private void Move(int source, int insertion)
    {
        int destination = insertion > source ? insertion - 1 : insertion;
        if (destination == source) return;
        serializedObject.Update();
        actions.MoveArrayElement(source, destination);
        bool wasExpanded = expanded[source];
        expanded.RemoveAt(source);
        expanded.Insert(destination, wasExpanded);
        selected = destination;
        ApplyStructure("컷씬 액션 순서 변경");
    }

    private void ApplyStructure(string undoName)
    {
        Undo.SetCurrentGroupName(undoName);
        serializedObject.ApplyModifiedProperties();
        // Rebind after the triggering UI event has completed so array paths cannot remain stale.
        root.schedule.Execute(() =>
        {
            if (root == null || target == null) return;
            serializedObject.Update();
            Rebuild();
            Select(selected, true);
        });
    }

    private void SetWaitMethod(int index, string method)
    {
        serializedObject.Update();
        var action = actions.GetArrayElementAtIndex(index);
        action.FindPropertyRelative("callFunction").boolValue = true;
        action.FindPropertyRelative("method").stringValue = method;
        serializedObject.ApplyModifiedProperties();
        Refresh();
    }

    private void SetType(int index, CutSceneActionType type)
    {
        serializedObject.Update();
        var action = actions.GetArrayElementAtIndex(index);
        var method = action.FindPropertyRelative("method");
        var previousType = ((CutSceneData)target).cutSceneActions[index].Type;
        action.FindPropertyRelative("callFunction").boolValue = type != CutSceneActionType.Dialogue;
        switch (type)
        {
            case CutSceneActionType.Wait:
                if (previousType != CutSceneActionType.Wait) method.stringValue = nameof(CutsceneManager.WaitUntil);
                break;
            case CutSceneActionType.Function:
                if (method.stringValue == nameof(CutsceneManager.WaitUntil) || method.stringValue == nameof(CutsceneManager.WaitUntilAndHide))
                    method.stringValue = "";
                break;
        }
        serializedObject.ApplyModifiedProperties();
        Refresh();
    }

    private void SetSprite(int index, Sprite sprite)
    {
        serializedObject.Update();
        var action = actions.GetArrayElementAtIndex(index);
        action.FindPropertyRelative("sprite").objectReferenceValue = sprite;
        action.FindPropertyRelative("clearSprite").boolValue = false;
        serializedObject.ApplyModifiedProperties();
        Refresh();
    }

    private void SetExpanded(bool value)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            expanded[i] = value;
            cards[i].Foldout.value = value;
        }
    }

    private void OnUndoRedo()
    {
        if (root == null) return;
        root.schedule.Execute(() =>
        {
            if (root == null || target == null) return;
            serializedObject.Update();
            Rebuild();
        });
    }

    private void Refresh()
    {
        var data = (CutSceneData)target;
        for (int i = 0; i < cards.Count; i++)
        {
            var action = data.cutSceneActions[i];
            var card = cards[i];
            string firstLine = (action.text ?? "").Replace('\n', ' ').Replace('\r', ' ');
            card.Type.SetValueWithoutNotify(TypeNames[(int)action.Type]);
            card.Sprite.SetValueWithoutNotify(action.DisplaySprite);
            card.Summary.text = action.Type == CutSceneActionType.Dialogue ? $"{action.name} · {firstLine}" : $"{action.clazz}.{action.method}()";
            switch (action.Type)
            {
                case CutSceneActionType.Dialogue:
                    card.Options.text = "대사 · 입력 대기";
                    break;
                case CutSceneActionType.Function:
                    card.Options.text = "함수 실행 · 자동 진행";
                    break;
                case CutSceneActionType.Wait:
                    card.Options.text = "외부 신호 대기 · " + (action.method == nameof(CutsceneManager.WaitUntilAndHide) ? "화면 숨기기" : "화면 유지");
                    break;
            }
            card.DialogueFields.style.display = action.Type == CutSceneActionType.Dialogue ? DisplayStyle.Flex : DisplayStyle.None;
            card.FunctionFields.style.display = action.Type != CutSceneActionType.Dialogue ? DisplayStyle.Flex : DisplayStyle.None;
            card.Method.style.display = action.Type == CutSceneActionType.Function ? DisplayStyle.Flex : DisplayStyle.None;
            card.WaitMode.style.display = action.Type == CutSceneActionType.Wait ? DisplayStyle.Flex : DisplayStyle.None;
            card.WaitHelp.style.display = card.WaitMode.style.display;
            card.WaitMode.SetValueWithoutNotify(action.method == nameof(CutsceneManager.WaitUntilAndHide) ? WaitModes[1] : WaitModes[0]);
            string warning = "";
            if (action.callFunction && (string.IsNullOrWhiteSpace(action.clazz) || string.IsNullOrWhiteSpace(action.method)))
                warning = "함수 실행에 필요한 대상 GameObject 이름과 메서드 이름을 모두 입력하세요.";
            card.Warning.text = warning;
            card.Warning.style.display = warning.Length > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            if (warning.Length > 0) card.Options.text += " · ⚠ 설정 확인";
            card.Root.EnableInClassList("selected", i == selected);
        }
        FilterCards();
        RefreshPreview();
    }

    private void FilterCards()
    {
        int visible = 0;
        var data = (CutSceneData)target;
        for (int i = 0; i < cards.Count; i++)
        {
            var action = data.cutSceneActions[i];
            bool matches = string.IsNullOrEmpty(search) || $"{action.name}\n{action.text}\n{action.clazz}\n{action.method}".IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
            cards[i].Root.style.display = matches ? DisplayStyle.Flex : DisplayStyle.None;
            if (matches) visible++;
        }
        foreach (var insertion in insertions)
            insertion.style.display = string.IsNullOrEmpty(search) ? DisplayStyle.Flex : DisplayStyle.None;
        countLabel.text = $"{visible} / {cards.Count} 액션" + (string.IsNullOrEmpty(search) ? " · ≡ 핸들로 순서를 변경합니다." : " · 검색 중에는 삽입·드래그가 숨겨집니다.");
    }

    private void Select(int index, bool scroll)
    {
        selected = Mathf.Clamp(index, 0, Mathf.Max(0, cards.Count - 1));
        for (int i = 0; i < cards.Count; i++) cards[i].Root.EnableInClassList("selected", i == selected);
        RefreshPreview();
        if (scroll && cards.Count > 0) list.ScrollTo(cards[selected].Root);
    }

    private void EvaluatePreview(int end, out string name, out Sprite sprite, out bool isLeft, out string text)
    {
        name = "";
        sprite = null;
        isLeft = false;
        text = "";
        var data = ((CutSceneData)target).cutSceneActions;
        for (int i = 0; i <= end; i++)
        {
            var action = data[i];
            if (action.Type != CutSceneActionType.Dialogue) continue;
            name = action.name;
            text = action.text;
            sprite = action.DisplaySprite;
            isLeft = action.isLeft;
        }
    }

    private void RefreshPreview()
    {
        bool hasActions = actions.arraySize > 0;
        previous.SetEnabled(hasActions && selected > 0);
        next.SetEnabled(hasActions && selected < actions.arraySize - 1);
        previewStatus.text = hasActions ? $"{selected + 1} / {actions.arraySize}" : "액션을 추가하세요";
        EvaluatePreview(hasActions ? selected : -1, out string name, out Sprite sprite, out bool isLeft, out string text);
        previewName.text = name ?? "";
        previewText.text = text ?? "";
        leftImage.sprite = isLeft ? sprite : null;
        rightImage.sprite = isLeft ? null : sprite;
        leftImage.style.visibility = sprite != null && isLeft ? Visibility.Visible : Visibility.Hidden;
        rightImage.style.visibility = sprite != null && !isLeft ? Visibility.Visible : Visibility.Hidden;
    }

    private void UpdateDrop(Vector2 position)
    {
        ClearDrop();
        if (!list.worldBound.Contains(position)) return;
        for (int i = 0; i < insertions.Count; i++)
        {
            if (position.y <= insertions[i].worldBound.center.y || i == insertions.Count - 1)
            {
                dropIndex = i;
                insertions[i].AddToClassList("drop-target");
                break;
            }
        }
        float delta = position.y < list.worldBound.yMin + 36 ? -18 : position.y > list.worldBound.yMax - 36 ? 18 : 0;
        list.scrollOffset += new Vector2(0, delta);
    }

    private void ClearDrop()
    {
        foreach (var insertion in insertions) insertion.RemoveFromClassList("drop-target");
        dropIndex = -1;
    }

    private sealed class ActionDrag : PointerManipulator
    {
        private readonly CutSceneDataEditor owner;
        private readonly int index;
        private bool active;
        private bool moved;
        private Vector2 start;

        public ActionDrag(CutSceneDataEditor owner, int index)
        {
            this.owner = owner;
            this.index = index;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(Down);
            target.RegisterCallback<PointerMoveEvent>(Move);
            target.RegisterCallback<PointerUpEvent>(Up);
            target.RegisterCallback<PointerCaptureOutEvent>(Cancel);
            target.RegisterCallback<KeyDownEvent>(KeyDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(Down);
            target.UnregisterCallback<PointerMoveEvent>(Move);
            target.UnregisterCallback<PointerUpEvent>(Up);
            target.UnregisterCallback<PointerCaptureOutEvent>(Cancel);
            target.UnregisterCallback<KeyDownEvent>(KeyDown);
        }

        private void Down(PointerDownEvent evt)
        {
            if (evt.button != 0 || !string.IsNullOrEmpty(owner.search)) return;
            active = true;
            moved = false;
            start = evt.position;
            target.focusable = true;
            target.Focus();
            target.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void Move(PointerMoveEvent evt)
        {
            if (!active) return;
            if (Vector2.Distance(start, evt.position) > 5) moved = true;
            if (moved) owner.UpdateDrop(evt.position);
            evt.StopPropagation();
        }

        private void Up(PointerUpEvent evt)
        {
            if (!active || evt.button != 0) return;
            int destination = owner.dropIndex;
            bool shouldMove = moved && destination >= 0;
            active = false;
            target.ReleasePointer(evt.pointerId);
            owner.ClearDrop();
            if (shouldMove) owner.Move(index, destination);
            evt.StopPropagation();
        }

        private void KeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Escape || !active) return;
            target.ReleasePointer(PointerId.mousePointerId);
            active = false;
            owner.ClearDrop();
            evt.StopPropagation();
        }

        private void Cancel(PointerCaptureOutEvent evt)
        {
            active = false;
            owner.ClearDrop();
        }
    }
}
