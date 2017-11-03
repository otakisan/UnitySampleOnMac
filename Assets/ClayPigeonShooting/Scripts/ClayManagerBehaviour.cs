using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor;
using UnityEngine;

public class ClayManagerBehaviour : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        this.originalClay = GameObject.Find("OriginalClaySphere");

        this.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateRemaingTime();
        this.GameoverIfNoTimeLeftAndLoadNextScene();
        this.ShootNewClay();
    }

    private GameObject originalClay;
    private const string targetClayTag = "ClaySphere";

    private bool ExistsTargetClay()
    {
        //return GameObject.FindGameObjectsWithTag(targetClayTag).Any(clay => clay.name != "OriginalClaySphere");
        return GroupNameDefition.all.SelectMany(groupName => GameObject.FindGameObjectsWithTag(groupName)).Count() > 0;
    }

    private System.Random randomForDirection = new System.Random();
    private int numsOfInstantiation = 2;
    public List<Material> targetMaterials;
    private void ShootNewClay()
    {
        if (this.ExistsTargetClay())
        {
            return;
        }

        var alreadyUses = new List<int>(this.numsOfInstantiation);
        for (int i = 0; i < this.numsOfInstantiation; i++)
        {
            var groupData = this.groups[this.randomForDirection.Next(this.groups.Count())];
            var newClay = Instantiate(this.originalClay);
            //newClay.name = "ClaySphere";
            //newClay.tag = targetClayTag;
            newClay.name = groupData.features[this.randomForDirection.Next(groupData.features.Count())].name;
            newClay.tag = groupData.name;
            newClay.GetComponent<Renderer>().material = groupData.material;
            var textMesh = newClay.GetComponentInChildren<TextMesh>();
            textMesh.text = groupData.name;
            textMesh.color = Color.white;
            newClay.transform.SetParent(this.originalClay.transform.parent);
            //newClay.GetComponent<Renderer>().material.n


            var direction = 0;
            do
            {
                direction = randomForDirection.Next(-1, 2);
            } while (alreadyUses.Exists(existing => existing == direction));

            newClay.transform.Translate(2 * direction, 1, 2);
            newClay.GetComponent<Rigidbody>().AddForce(new Vector3(direction * 20, 20, 20), ForceMode.VelocityChange);
            alreadyUses.Add(direction);
        }

        //this.currentClay = newClay;
    }

    public float gameTime;
    private void GameoverIfNoTimeLeftAndLoadNextScene()
    {
        var remaining = this.gameTime;
        if(remaining <= 0)
        {
            SharedDataAmongScenes.finalScore = ScoreTextBehaviour.Instance.GetScore();
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
        }
    }

    private void UpdateRemaingTime()
    {
        this.gameTime -= Time.deltaTime;
        GameObject.Find("RemainingTimeText").GetComponent<UnityEngine.UI.Text>().text = this.gameTime.ToString();
    }

    //private float RemainingTime
    //{
    //    get
    //    {
    //        return this.gameTime - Time.deltaTime;
    //    }
    //}

    public static GameObject FindTarget()
    {
        return GameObject.Find(targetClayTag);
    }

    public static GameObject FindClosestTarget()
    {
        //var targets = GameObject.FindGameObjectsWithTag(targetClayTag).Where(clay => clay.name != "OriginalClaySphere");
        var targets = GroupNameDefition.all.SelectMany(groupName => GameObject.FindGameObjectsWithTag(groupName));
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 currentRotation = Camera.main.transform.eulerAngles;
        foreach (GameObject target in targets)
        {
            var targetRotation = Quaternion.LookRotation(target.transform.position - Camera.main.transform.position).eulerAngles;
            Vector3 diff = targetRotation - currentRotation;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = target;
                distance = curDistance;
            }
        }
        return closest;
    }

    public static ClayManagerBehaviour GetClayManager()
    {
        return GameObject.Find("ClayManager").GetComponent<ClayManagerBehaviour>();
    }

    public List<IDEFeatureGroupData> groups;
    public List<IDEDevlopmentProdure> procedureItems;

    public class GroupNameDefition
    {
        public const string Dictionary = "Dictionary";
        public const string Generator = "Generator";
        public const string FormDesignAssist = "FormDesignAssist";
        public const string CodingAssist = "CodingAssist";

        public static readonly string[] all = { Dictionary, Generator, FormDesignAssist, CodingAssist };
    }

    private string lastHitGroupName;
    public void HitTargetClay(string groupName)
    {
        //bool valid = true;
        //if(!string.IsNullOrEmpty(this.lastHitGroupName))
        //{
        //    valid = this.procedureItems.FirstOrDefault(group => group.groupName == this.lastHitGroupName)?.nextGroupNames.Any(nextGroupName => nextGroupName == groupName) ?? false;
        //}

        //bool valid = this.IsActionValid(groupName, CompleteAction.Hit);

        //this.lastHitGroupName = groupName;


        //Debug.Log(valid ? "妥当なヒット" : "不正なヒット");

        GameObject.Find("ScoreText").GetComponent<ScoreTextBehaviour>().UpdateScore(groupName, CompleteAction.Hit);
        this.lastHitGroupName = groupName;

        this.UpdateDevFlowText();
        //var scoreTextLabel = GameObject.Find("ScoreText").GetComponent<UnityEngine.UI.Text>();
        //int score = 0;
        //if(int.TryParse(scoreTextLabel.text, out score))
        //{
        //    scoreTextLabel.text = (score + 100 * (valid ? 1 : -1)).ToString();
        //}
    }

    private void UpdateDevFlowText()
    {
        var flowText = $"Done: {System.Environment.NewLine}{this.lastHitGroupName ?? "(Nothing)"}" + System.Environment.NewLine + System.Environment.NewLine + "Next:";
        flowText = this.procedureItems.FirstOrDefault(item => item.groupName == this.lastHitGroupName)?
                       .nextGroupNames.Aggregate(flowText, (prev, cur) => prev + System.Environment.NewLine + cur) ?? flowText;

        GameObject.Find("DevelopmentFlowText").GetComponent<DevelopmentFlowTextBehaviour>()
                  .SetText(flowText.EndsWith("Next:", System.StringComparison.CurrentCulture) ? flowText + System.Environment.NewLine + this.procedureItems.FirstOrDefault()?.groupName ?? "(Nothing)" : flowText);
        
    }

    public bool IsActionValid(string groupName, CompleteAction action)
    {
        bool valid = true;
        if (!string.IsNullOrEmpty(this.lastHitGroupName) && this.procedureItems.FirstOrDefault(item => item.groupName == this.lastHitGroupName)?.nextGroupNames.Count() > 0)
        {
            valid = this.procedureItems.FirstOrDefault(group => group.groupName == this.lastHitGroupName)?.nextGroupNames.Any(nextGroupName => nextGroupName == groupName) ?? false;
        }
        else
        {
            valid = this.procedureItems.FirstOrDefault()?.groupName == groupName;
        }

        return action == CompleteAction.Hit ? valid : !valid;
    }

    private void Initialize()
    {
        //this.groups = new List<IDEFeatureGroupData>()
        //{
        //    new IDEFeatureGroupData(){name = GroupNameDefition.Dictionary, features= new List<IDEFeatureData>{ new IDEFeatureData{name = "ProgramDictionary"}} ,material = AssetDatabase.LoadAssetAtPath<Material>("Assets/ClayPigeonShooting/Materials/DictionaryGroupClayMaterial.mat")},
        //    new IDEFeatureGroupData(){name = GroupNameDefition.Generator, features= new List<IDEFeatureData>{ new IDEFeatureData{name = "InheritsFile"}, new IDEFeatureData{name="ProgramSourceGenerator"}} , material = AssetDatabase.LoadAssetAtPath<Material>("Assets/ClayPigeonShooting/Materials/GeneratorGroupClayMaterial.mat")},
        //    new IDEFeatureGroupData(){name = GroupNameDefition.FormDesignAssist, features= new List<IDEFeatureData>{ new IDEFeatureData{name = "FormGeneratorAssist"}} , material = AssetDatabase.LoadAssetAtPath<Material>("Assets/ClayPigeonShooting/Materials/ClayMaterial.mat")},
        //    new IDEFeatureGroupData(){name = GroupNameDefition.CodingAssist, features= new List<IDEFeatureData>{ new IDEFeatureData{name = "LogicCustomizeAssist"}} , material = AssetDatabase.LoadAssetAtPath<Material>("Assets/ClayPigeonShooting/Materials/CodingAssistGroupClayMaterial.mat")},
        //};

        //this.procedureItems = new List<IDEDevlopmentProdure>()
        //{
        //    new IDEDevlopmentProdure(){ groupName = "Dictionary", nextGroupNames = new List<string>(){"Generator"}},
        //    new IDEDevlopmentProdure(){ groupName = "Generator", nextGroupNames = new List<string>(){"FormDesignAssist", "CodingAssist"}},
        //    new IDEDevlopmentProdure(){ groupName = "FormDesignAssist", nextGroupNames = new List<string>(){"Dictionary", "CodingAssist"}},
        //    new IDEDevlopmentProdure(){ groupName = "CodingAssist", nextGroupNames = new List<string>()},
        //};

        //foreach (var def in GroupNameDefition.all)
        //{
        //    this.AddTag(def);
        //}

        SharedDataAmongScenes.Reset();
        this.UpdateDevFlowText();
    }

    //void AddTag(string tagname)
    //{
    //    UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
    //    if ((asset != null) && (asset.Length > 0))
    //    {
    //        SerializedObject so = new SerializedObject(asset[0]);
    //        SerializedProperty tags = so.FindProperty("tags");

    //        for (int i = 0; i < tags.arraySize; ++i)
    //        {
    //            if (tags.GetArrayElementAtIndex(i).stringValue == tagname)
    //            {
    //                return;
    //            }
    //        }

    //        int index = tags.arraySize;
    //        tags.InsertArrayElementAtIndex(index);
    //        tags.GetArrayElementAtIndex(index).stringValue = tagname;
    //        so.ApplyModifiedProperties();
    //        so.Update();
    //    }
    //}
    [System.Serializable]
    public class IDEDevlopmentProdure
    {
        public string groupName;
        public List<string> nextGroupNames;
    }

    [System.Serializable]
    public class IDEFeatureGroupData
    {
        public string name;
        public Material material;
        public List<IDEFeatureData> features;
    }

    [System.Serializable]
    public class IDEFeatureData
    {
        public string name;
    }

    // カスタムのエディターウィンドウを定義したり
    // もしくは、設定ファイルを扱いやすい仕組みを導入したり
    // とりあえず、後回しにする
#if UNITY_EDITOR
    //[CustomEditor(typeof(ClayManagerBehaviour))]
    //[CanEditMultipleObjects]
    //public class ClayManagerBehaviourEditor : Editor
    //{
    //    bool folding = false;
    //    bool folding2 = false;

    //    public override void OnInspectorGUI()
    //    {
    //        var controller = target as ClayManagerBehaviour;

    //        //var field = EditorGUILayout.ObjectField("procedure", controller.procedure, typeof(IDEDevlopmentProdure), false, null) as IDEDevlopmentProdure;
    //        //chara.m_name = EditorGUILayout.TextField("名前", chara.m_name);

    //        List<IDEDevlopmentProdure> list = controller.procedureItems.Where(item => !string.IsNullOrEmpty(item.groupName)).ToList();

    //        int i, len = list.Count;

    //        // 折りたたみ表示
    //        if (folding = EditorGUILayout.Foldout(folding, "procedureItems"))
    //        {
    //            EditorGUI.indentLevel++;

    //            // リスト表示
    //            for (i = 0; i < len; ++i)
    //            {
    //                controller.procedureItems[i].groupName = EditorGUILayout.TextField("group name " + i.ToString(), controller.procedureItems[i].groupName);

    //                if (folding2 = EditorGUILayout.Foldout(folding, "next group name"))
    //                {
    //                    EditorGUI.indentLevel++;
    //                    for (int j = 0; j < controller.procedureItems[i].nextGroupNames.Count(); j++)
    //                    {
    //                        controller.procedureItems[i].nextGroupNames[j] = EditorGUILayout.TextField(j.ToString(), controller.procedureItems[i].nextGroupNames[j]);
    //                    }

    //                    EditorGUILayout.LabelField("追加");
    //                    var newNextGroupName = EditorGUILayout.TextField("nextGroupName", string.Empty);
    //                    if (!string.IsNullOrEmpty(newNextGroupName))
    //                    {
    //                        controller.procedureItems[i].nextGroupNames.Add(newNextGroupName);
    //                    }
    //                    EditorGUI.indentLevel--;
    //                }

    //                EditorGUILayout.Separator();
    //            }

    //            EditorGUILayout.LabelField("追加");
    //            EditorGUI.indentLevel++;
    //            var newGroupName = EditorGUILayout.TextField("groupName", string.Empty);
    //            //var newGroupSize = EditorGUILayout.IntField("サイズ", 0);
    //            EditorGUI.indentLevel--;
    //            if(!string.IsNullOrEmpty(newGroupName))
    //            {
    //                var newItem = new IDEDevlopmentProdure()
    //                {
    //                    groupName = newGroupName,
    //                    nextGroupNames = new List<string>()
    //                };

    //                //for (int k = 0; k < newGroupSize; k++)
    //                //{
    //                //    newItem.nextGroupNames.Add(string.Empty);
    //                //}

    //                list.Add(newItem);
    //            }

    //            EditorGUI.indentLevel--;

    //            controller.procedureItems = list;
    //            //var go = EditorGUILayout.ObjectField("追加", null, typeof(IDEDevlopmentProdure), true) as IDEDevlopmentProdure;
    //            //if (go != null)
    //                //list.Add(go);
    //        }

    //        //EditorGUILayout.LabelField("procedure");
    //        //EditorGUI.indentLevel++;

    //        //EditorGUILayout.LabelField("current group");
    //        //EditorGUI.indentLevel++;
    //        //EditorGUILayout.TextField("name", controller.procedure.current.name);
    //        //EditorGUILayout.ObjectField("material", controller.procedure.current.material, typeof(Material), true);
    //        //EditorGUI.indentLevel--;

    //        //if (folding2 = EditorGUILayout.Foldout(folding, "next groups"))
    //        //{
    //        //    EditorGUI.indentLevel++;

    //        //    // リスト表示
    //        //    for (i = 0; i < controller.procedure.next.; ++i)
    //        //    {
    //        //        list[i] = EditorGUILayout.ObjectField(list[i], typeof(Material), true) as Material;
    //        //    }

    //        //    Material go = EditorGUILayout.ObjectField("追加", null, typeof(Material), true) as Material;
    //        //    if (go != null)
    //        //        list.Add(go);
                
    //        //    EditorGUI.indentLevel--;
               
    //        //}
    //        //EditorGUILayout.LabelField("next groups");

    //        //EditorGUI.indentLevel--;

    //        // -- 友達 --
    //        //List<Material> list = controller.targetMaterials;
    //        //int i, len = list.Count;

    //        //// 折りたたみ表示
    //        //if (folding = EditorGUILayout.Foldout(folding, "targetMaterials"))
    //        //{
    //        //    // リスト表示
    //        //    for (i = 0; i < len; ++i)
    //        //    {
    //        //        list[i] = EditorGUILayout.ObjectField(list[i], typeof(Material), true) as Material;
    //        //    }

    //        //    Material go = EditorGUILayout.ObjectField("追加", null, typeof(Material), true) as Material;
    //        //    if (go != null)
    //        //        list.Add(go);
    //        //}
    //    }
    //}

    //[CustomEditor(typeof(IDEDevlopmentProdure))]
    //public class IDEDevlopmentProdureEditor : Editor
    //{
    //    bool folding = false;

    //    public override void OnInspectorGUI()
    //    {
    //        var controller = target as IDEDevlopmentProdure;
    //        // -- 友達 --
    //        List<IDEDevlopmentProdure> list = controller.next;
    //        int i, len = list.Count;

    //        // 折りたたみ表示
    //        if (folding = EditorGUILayout.Foldout(folding, "next groups"))
    //        {
    //            // リスト表示
    //            for (i = 0; i < len; ++i)
    //            {
    //                list[i] = EditorGUILayout.ObjectField(list[i], typeof(IDEDevlopmentProdure), true) as IDEDevlopmentProdure;
    //            }

    //            var go = EditorGUILayout.ObjectField("追加", null, typeof(IDEDevlopmentProdure), true) as IDEDevlopmentProdure;
    //            if (go != null)
    //                list.Add(go);
    //        }
    //    }
    //}
#endif
}

public enum CompleteAction
{
    //None,
    Hit,
    Away
}

public static class SharedDataAmongScenes
{
    public static int finalScore;

    public static void Reset()
    {
        finalScore = 0;
    }
}