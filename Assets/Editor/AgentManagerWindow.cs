using UnityEngine;
using UnityEditor;
using System.IO;
public class AgentManagerWindow : EditorWindow
{
    private AgentList _agentListObject;         //List of Agents
    private AgentCharacter _selectedAgent;      //Currently Selected Agent

    private Vector2 _scrollPosLeft;
    private Vector2 _scrollPosRight;            
    private string _newAgentName = "NewAgent";  //Default New Agent Name

    private bool _abilitiesFoldout = true;      // Open/Close State for the Ability foldouts section

    private const float SidebarWidth = 220f;    // max sidebar width
    private const float MaxFieldWidth = 450f;   // max field width
    private const string BasePath = "Assets/Agents";

    [MenuItem("Tools/Agent Manager")]
    public static void ShowWindow()
    {
        GetWindow<AgentManagerWindow>("Agent Manager");
    }
    private void OnEnable()
    {
        FindAgentList();
    }
    private void OnGUI()
    {
        DrawTopBar();
        if (_agentListObject == null)
        {
            EditorGUILayout.HelpBox("Please assign or create an 'AgentList' ScriptableObject.", MessageType.Info);
            return;
        }
        EditorGUILayout.BeginHorizontal();
        DrawSidebar();
        GUILayout.BeginVertical(new GUIStyle() { padding = new RectOffset(10, 10, 10, 10) });// give main editor area  a little padding
        DrawSelectedAgent();
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
    // ---------------- UI SECTIONS ---------------- //
    private void DrawTopBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Database Configuration");
        _agentListObject = (AgentList)EditorGUILayout.ObjectField(_agentListObject, typeof(AgentList), false, GUILayout.Width(200));
        if (GUILayout.Button("Refresh List", EditorStyles.toolbarButton, GUILayout.Width(80))) FindAgentList();
        EditorGUILayout.EndHorizontal();
    }
    private void DrawSidebar()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(SidebarWidth));
        GUILayout.Label("Agents", EditorStyles.boldLabel);

        _scrollPosLeft = EditorGUILayout.BeginScrollView(_scrollPosLeft, "box");
        if (_agentListObject != null && _agentListObject.list != null)
        {
            for (int i = 0; i < _agentListObject.list.Count; i++)
            {
                AgentCharacter agent = _agentListObject.list[i];
                if (agent == null) continue;

                GUIStyle style = (agent == _selectedAgent) ? new GUIStyle(GUI.skin.button) { normal = GUI.skin.button.active } : GUI.skin.button;
                if (GUILayout.Button(agent.name, style))
                {
                    _selectedAgent = agent;
                    GUI.FocusControl(null);
                }
            }
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        GUILayout.Label("Create New Agent", EditorStyles.boldLabel);
        _newAgentName = EditorGUILayout.TextField("Agent Name", _newAgentName);
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Agent", GUILayout.Height(30))) CreateNewAgent();
        EditorGUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }
    private void DrawSelectedAgent()
    {
        EditorGUILayout.BeginVertical();

        if (_selectedAgent != null)
        {
            _scrollPosRight = EditorGUILayout.BeginScrollView(_scrollPosRight);

            //GUILayout.Label($"Editing: {_selectedAgent.name}", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            SerializedObject so = new SerializedObject(_selectedAgent);
            so.Update();

            SerializedProperty prop = so.GetIterator();
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                if (prop.name == "m_Script") { enterChildren = false; continue; }//hide the "Script" field from editor window

                //Draw Artwork with Preview 
                if (prop.name == "panelArtwork" || prop.name == "panelTransparentArtwork")
                {
                    DrawPropertyWithPreview(prop);
                }
                // Handle Abilities with Custom List
                else if (prop.name == "abilities")
                {
                    DrawAbilitiesList(prop);
                }
                //Standard Fields (Colors, Enums, etc.)
                else
                {
                    EditorGUILayout.PropertyField(prop, true, GUILayout.MaxWidth(MaxFieldWidth));
                }
                enterChildren = false;
            }

            if (so.ApplyModifiedProperties())   EditorUtility.SetDirty(_selectedAgent);//does not preserve undo state;

            // Delete Agent Button
            EditorGUILayout.Space(20);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Delete Agent", GUILayout.Height(30), GUILayout.MaxWidth(MaxFieldWidth)))
            {
                if (EditorUtility.DisplayDialog("Delete Agent", $"Delete {_selectedAgent.name}?", "Yes", "No")) DeleteAgent(_selectedAgent);
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndScrollView();
        }
        else //List is empty
            GUILayout.Label("List is currently empty.add a new agent", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndVertical();
    }
    // ---------------- CUSTOM DRAWERS ---------------- //
    private void DrawPropertyWithPreview(SerializedProperty prop)
    {
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(MaxFieldWidth));
        // Draw the field itself
        EditorGUILayout.PropertyField(prop);

        Sprite sprite = prop.objectReferenceValue as Sprite;
        if (sprite != null)
        {
            Texture2D previewTex = AssetPreview.GetAssetPreview(sprite);
            if (previewTex != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(previewTex, GUILayout.Width(128), GUILayout.Height(128));
                GUILayout.FlexibleSpace(); // Keep trailing space to push it left
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }
    private void DrawAbilitiesList(SerializedProperty abilitiesProp)
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Abilities Configuration", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box", GUILayout.Width(MaxFieldWidth));

        _abilitiesFoldout = EditorGUILayout.Foldout(_abilitiesFoldout, $"Abilities List ({abilitiesProp.arraySize})", true);

        if (_abilitiesFoldout)
        {
            for (int i = 0; i < abilitiesProp.arraySize; i++)
            {
                SerializedProperty element = abilitiesProp.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(element, GUIContent.none);

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    abilitiesProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                // INLINE EDITING
                AbilityData abilityData = element.objectReferenceValue as AbilityData;
                if (abilityData != null)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Internal Data:", EditorStyles.miniBoldLabel);

                    SerializedObject abilitySO = new SerializedObject(abilityData);
                    abilitySO.Update();

                    // Apply max width to ability name/desc too for consistency
                    EditorGUILayout.PropertyField(abilitySO.FindProperty("abilityName"), GUILayout.MaxWidth(MaxFieldWidth));
                    EditorGUILayout.PropertyField(abilitySO.FindProperty("description"), GUILayout.MaxWidth(MaxFieldWidth));

                    SerializedProperty abIcon = abilitySO.FindProperty("icon");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(abIcon);
                    if (abIcon.objectReferenceValue != null)
                    {
                        Texture2D iconTex = AssetPreview.GetAssetPreview(abIcon.objectReferenceValue);
                        if (iconTex) GUILayout.Label(iconTex, GUILayout.Width(40), GUILayout.Height(40));
                    }
                    GUILayout.FlexibleSpace(); // Ensure small icon doesn't stretch
                    EditorGUILayout.EndHorizontal();

                    if (abilitySO.ApplyModifiedProperties())
                    {
                        EditorUtility.SetDirty(abilityData);
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                GUILayout.Space(2);
            }

            GUILayout.Space(10);

            GUI.backgroundColor = Color.green;
            // Constrain width of button too for tidiness
            if (GUILayout.Button("+ Create New Ability", GUILayout.MaxWidth(MaxFieldWidth)))
            {
                CreateAbilityForSelectedAgent(abilitiesProp);
            }
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.EndVertical();
    }
    // ---------------- LOGIC FUNCTIONS ---------------- //
    private void CreateAbilityForSelectedAgent(SerializedProperty listProp)
    {
        string agentPath = AssetDatabase.GetAssetPath(_selectedAgent);
        string agentFolder = Path.GetDirectoryName(agentPath);
        string abilityFolder = Path.Combine(agentFolder, "Abilities");

        if (!AssetDatabase.IsValidFolder(abilityFolder))
        {
            AssetDatabase.CreateFolder(agentFolder, "Abilities");
        }

        AbilityData newAbility = ScriptableObject.CreateInstance<AbilityData>();
        newAbility.name = $"Ability_{_selectedAgent.name}_{listProp.arraySize + 1}";
        newAbility.abilityName = "New Ability";

        string assetPath = Path.Combine(abilityFolder, newAbility.name + ".asset");
        // Ensure unique name if file already exists
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        AssetDatabase.CreateAsset(newAbility, assetPath);

        listProp.arraySize++;
        SerializedProperty newElement = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
        newElement.objectReferenceValue = newAbility;

        listProp.serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }
    private void CreateNewAgent()
    {
        if (string.IsNullOrEmpty(_newAgentName)) return;

        if (!AssetDatabase.IsValidFolder("Assets/Agents")) AssetDatabase.CreateFolder("Assets", "Agents");

        string agentFolderPath = $"{BasePath}/{_newAgentName}";
        if (!AssetDatabase.IsValidFolder(agentFolderPath)) AssetDatabase.CreateFolder(BasePath, _newAgentName);

        AgentCharacter newAgent = ScriptableObject.CreateInstance<AgentCharacter>();
        newAgent.name = _newAgentName;

        string assetPath = $"{agentFolderPath}/{_newAgentName}.asset";
        if (AssetDatabase.LoadAssetAtPath<AgentCharacter>(assetPath) != null) return;

        AssetDatabase.CreateAsset(newAgent, assetPath);

        if (_agentListObject.list == null) _agentListObject.list = new System.Collections.Generic.List<AgentCharacter>();
        _agentListObject.list.Add(newAgent);

        EditorUtility.SetDirty(_agentListObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        _selectedAgent = newAgent;
        _newAgentName = "";
        GUI.FocusControl(null);
    }
    private void DeleteAgent(AgentCharacter agent)
    {
        if (_agentListObject.list.Contains(agent))
        {
            _agentListObject.list.Remove(agent);
            EditorUtility.SetDirty(_agentListObject);
        }

        string path = AssetDatabase.GetAssetPath(agent);
        AssetDatabase.DeleteAsset(path);

        string folderPath = Path.GetDirectoryName(path);
        if (Directory.GetFiles(folderPath).Length == 0) AssetDatabase.DeleteAsset(folderPath);

        _selectedAgent = null;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void FindAgentList()    //Finds AgentList ScriptableObject
    {
        string[] searchFolders = new[] { "Assets/Agents" };
        string[] guids = AssetDatabase.FindAssets("t:AgentList", searchFolders);
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _agentListObject = AssetDatabase.LoadAssetAtPath<AgentList>(path);
        }
    }
}