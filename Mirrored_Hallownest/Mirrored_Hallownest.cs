using Modding;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;
using UCamera = UnityEngine.Camera;
using InControl;
using GlobalEnums;
using System;
using TMPro;

namespace Mirrored_Hallownest
{
    public class isFlipped : MonoBehaviour
    {
        public bool flipped = false;
    }
    public class isFlippedText : MonoBehaviour
    {
        public float normalPos, normalScale;
        public bool flipped = false;
        public void Update()
        {
            //transform.RotateAround(transform.position - (Vector3)GetComponent<TextContainer>().rect.center, Vector3.up, 180 * Time.deltaTime);
            
        }
        public void flipText()
        {
            //if (flipped) return;

            RectTransform rectTransform = GetComponent<RectTransform>();
            TextMeshPro textMeshPro = GetComponent<TextMeshPro>();

            //transform.localScale.Scale(new Vector3(-1, 1, 1));
            //RectTransformUtility.FlipLayoutOnAxis(rectTransform, 0, false, true);

            //rectTransform.anchorMin = new Vector2((rectTransform.anchorMin.x - rectTransform.rect.center.x) * -1, rectTransform.anchorMin.y);
            //rectTransform.anchorMax = new Vector2((rectTransform.anchorMax.x - rectTransform.rect.center.x) * -1, rectTransform.anchorMax.y);
            /*textMeshPro.font
            foreach(TMP_Glyph glyph in textMeshPro.font.characterDictionary)
            {

            }*/

            //rectTransform.RotateAround(rectTransform.anchoredPosition3D, Vector3.up, 180);

            //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, -1 * rectTransform.GetScaleX());
            //rectTransform.localPosition += Vector3.right * rectTransform.rect.width;
            //RectTransformUtility.FlipLayoutOnAxis(GetComponent<RectTransform>(), 0, false, true);
            //transform.localPosition += new Vector3(GetComponent<TextMeshPro>().bounds.center.x, 0, 0);
            /*rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale.Scale(new Vector3(-1, 1, 1));
            rectTransform.anchoredPosition = */
            //RectTransformUtility.FlipLayoutOnAxis(rectTransform, 0, true, true);
            //transform.localScale.Scale(new Vector3(-1, 1, 1));
            //transform.localPosition -= GetComponent<RectTransform>().rect.width * Vector3.right;

            flipped = true;
        }
    }
    public class Mirrored_Hallownest : Mod
    {
        internal static Mirrored_Hallownest Instance;

        public string[] UnflippedScenes = new string[] {
            "Pre_Menu_Intro",
            "Menu_Title",
            "Opening_Sequence",
        };

        private Matrix4x4 _reflectMatrix = Matrix4x4.identity;
        private Matrix4x4 _reflectMatrixBlur = Matrix4x4.identity;
        private bool flipPrompt = false;
        private PlayerAction leftAction;
        private PlayerAction rightAction;
        //public override List<ValueTuple<string, string>> GetPreloadNames()
        //{
        //    return new List<ValueTuple<string, string>>
        //    {
        //        new ValueTuple<string, string>("White_Palace_18", "White Palace Fly")
        //    };
        //}

        public Mirrored_Hallownest() : base("Mirrored Hallownest")
        {
            Instance = this;
        }

        public override string GetVersion() => "0.3.0.0";

        public void Unload() //i planned for this to be a togglable mod but i couldn't make it work properly
        {
            On.tk2dCamera.UpdateCameraMatrix -= OnUpdateCameraMatrix;
            On.GameCameras.StartScene -= OnNewSceneCam;
            On.InputHandler.ActionButtonToPlayerAction -= OnAction;
            flipPrompt = false;
            //On.HeroController.GetCState -= OnCState;
            if (hasBlurCam(GameCameras.instance))
            {
                FlipUCam(GameCameras.instance.tk2dCam.transform.GetComponentInChildren<UCamera>());
            }
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;
            On.tk2dCamera.UpdateCameraMatrix += OnUpdateCameraMatrix;
            On.GameCameras.StartScene += OnNewSceneCam;

            //On.InputHandler.ActionButtonToPlayerAction += OnAction;
            //On.InputHandler.AttachHeroController
            InputHandler.Instance.inputActions.moveVector.InvertXAxis = true;
            leftAction = InputHandler.Instance.inputActions.left;
            rightAction = InputHandler.Instance.inputActions.right;
            InputHandler.Instance.inputActions.left = rightAction;
            InputHandler.Instance.inputActions.right = leftAction;
            //On.InputHandler.Update += InputHandler_Update;
            //On.InputHandler.AddKeyBinding += InputHandler_AddKeyBinding;

            On.ObjectPool.Spawn_GameObject_Transform_Vector3_Quaternion += OnSpawnObject;
            //On.GameMap.Update += GameMapUpdate;
            flipPrompt = true;

            Log("Initialized");
        }

        private void InputHandler_AddKeyBinding(On.InputHandler.orig_AddKeyBinding orig, PlayerAction action, string savedBinding)
        {
            orig(action, savedBinding);

            if (action.Name == leftAction.Name) leftAction = action;
            if (action.Name == rightAction.Name) rightAction = action;
        }

        private void InputHandler_Update(On.InputHandler.orig_Update orig, InputHandler self)
        {
            orig(self);

            InputHandler.Instance.inputActions.left = rightAction;
            InputHandler.Instance.inputActions.right = leftAction;
        }

        private void GameMapUpdate(On.GameMap.orig_Update orig, GameMap self)
        {
            orig(self);

            float scaleX = self.transform.GetScaleX();
            if (scaleX >= 0)
            {
                self.transform.SetScaleX(scaleX * -1);
                
            }
            TextMeshPro[] texts = self.GetComponentsInChildren<TextMeshPro>(true);
            foreach (TextMeshPro textMeshPro in texts)
            {
                isFlippedText flipped = textMeshPro.GetComponent<isFlippedText>();
                if (flipped == null)
                {
                    textMeshPro.gameObject.AddComponent<isFlippedText>();
                    flipped = textMeshPro.GetComponent<isFlippedText>();
                    flipped.normalPos = textMeshPro.transform.localPosition.x;
                    flipped.normalScale = textMeshPro.transform.localScale.x;
                }
                if (Input.GetKeyDown(KeyCode.H))flipped.flipText();
                //textMeshPro.transform.localPosition = new Vector3(-1 * Mathf.Sign(flipped.normalPos), textMeshPro.transform.localPosition.y, textMeshPro.transform.localPosition.z);
                //textMeshPro.transform.localScale = new Vector3(-1 * Mathf.Sign(flipped.normalScale), textMeshPro.transform.localScale.y, textMeshPro.transform.localScale.z);
                //textMeshPro.isRightToLeftText = true;
                /*Mesh mesh = textMeshPro.GetComponent<MeshFilter>().sharedMesh;

                Vector3[] vertices = mesh.vertices;
                for (int p = 0; p < vertices.Length; p++)
                {
                    vertices[p].Scale(new Vector3(-1, 1, 1));
                }
                mesh.vertices = vertices;
                mesh.RecalculateNormals();*/
                //textMeshPro.transform.RotateAround(textMeshPro.bounds.center + textMeshPro.transform.position, Vector3.up, 180);
            }
        }

        private PlayerAction OnAction(On.InputHandler.orig_ActionButtonToPlayerAction orig, InputHandler self, HeroActionButton actionButtonType)
        {
            HeroActionButton newButton = actionButtonType;
            if (actionButtonType == HeroActionButton.LEFT) newButton = HeroActionButton.RIGHT;
            else if (actionButtonType == HeroActionButton.RIGHT) newButton = HeroActionButton.LEFT;
            return orig(self, newButton);

        }

        void exchangeInputs(PlayerAction a, PlayerAction b)
        {
            for (int i = 0; i < Mathf.Min(a.Bindings.Count, b.Bindings.Count); i++)
            {
                BindingSource aBinding = a.Bindings[i];
                a.ReplaceBinding(a.Bindings[i], b.Bindings[i]);
                b.ReplaceBinding(b.Bindings[i], aBinding);
            }
            bool aHigh = a.Bindings.Count > b.Bindings.Count;
            for (int i = Mathf.Min(a.Bindings.Count, b.Bindings.Count) - 1; i < (aHigh ? a.Bindings.Count - b.Bindings.Count : b.Bindings.Count - a.Bindings.Count); i++)
            {
                if (aHigh)
                {
                    b.AddBinding(a.Bindings[i]);
                    a.RemoveBinding(a.Bindings[i]);
                }
                else
                {
                    a.AddBinding(b.Bindings[i]);
                    b.RemoveBinding(b.Bindings[i]);
                }
            }
        }


        private void OnNewSceneCam(On.GameCameras.orig_StartScene orig, GameCameras self)
        {
            
            orig(self);

            foreach(UCamera cam in self.tk2dCam.GetComponentsInChildren<UCamera>())
            {
                if (cam.gameObject.GetComponent<isFlipped>() == null) cam.gameObject.AddComponent<isFlipped>();
                if (cam.gameObject.GetComponent<isFlipped>().flipped) return;

                FlipUCam(cam);
                cam.gameObject.GetComponent<isFlipped>().flipped = true;
            }

            /*if (!hasBlurCam(self))
            {
                return;
            }
            if (self.tk2dCam.transform.GetComponentInChildren<isFlipped>() == null) self.tk2dCam.gameObject.AddComponent<isFlipped>();
            if (self.tk2dCam.transform.GetComponentInChildren<isFlipped>().flipped)
            {
                return;
            }

            self.tk2dCam.transform.GetComponentInChildren<isFlipped>().flipped = true;
            FlipUCam(self.tk2dCam.transform.GetComponentInChildren<UCamera>());
            orig(self);*/

            
        }
        private void OnNewSceneCam(GameCameras self)
        {
            /*string activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (Array.Exists<string>(UnflippedScenes, Name => activeScene == Name)) return;*/

            if (!GameManager.instance.IsGameplayScene()) return;

            if (!hasBlurCam(self))
            {
                return;
            }
            if (self.tk2dCam.transform.GetComponentInChildren<isFlipped>() == null) self.tk2dCam.gameObject.AddComponent<isFlipped>();
            if (self.tk2dCam.transform.GetComponentInChildren<isFlipped>().flipped)
            {
                return;
            }

            self.tk2dCam.transform.GetComponentInChildren<isFlipped>().flipped = true;
            FlipUCam(self.tk2dCam.transform.GetComponentInChildren<UCamera>());
        }

        private GameObject OnSpawnObject(On.ObjectPool.orig_Spawn_GameObject_Transform_Vector3_Quaternion orig, GameObject prefab, Transform parent, Vector3 pos, Quaternion rot)
        {
            prefab = orig(prefab, parent, pos, rot);
            TextMeshPro[] prefabPrompt;
            prefabPrompt = prefab.gameObject.GetComponentsInChildren<TextMeshPro>(true);
            foreach (TextMeshPro prompt in prefabPrompt)
            {
                prompt.transform.localScale = new Vector3(flipPrompt ? -1 : 1, 1, 1);
            }
            /*if (prefabPrompt != null)
            {
                prefab.transform.localScale = new Vector3(flipPrompt ? -1 : 1, 1, 1);
            }*/
            return prefab;
        }

        private void OnUpdateCameraMatrix(On.tk2dCamera.orig_UpdateCameraMatrix orig, tk2dCamera self)
        {
            orig(self);

            /*string activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (Array.Exists<string>(UnflippedScenes, Name => activeScene == Name)) return;*/

            if (!GameManager.instance.IsGameplayScene()) return;

            // Can't use ?. on a Unity type because they override == to null.
            if (GameCameras.instance == null || GameCameras.instance.tk2dCam == null)
                return;

            UCamera cam = self.GetComponent<UCamera>();

            if (cam == null)
                return;

            Matrix4x4 p = cam.projectionMatrix;

            //_reflectMatrix[1, 1] = -1;
            //p *= _reflectMatrix;
            _reflectMatrix[0, 0] = -1;
            p *= _reflectMatrix;

            cam.projectionMatrix = p;


            /*UCamera blurCam = cam.transform.GetComponentInChildren<UCamera>();
            if (blurCam == null)
                return;
            p = blurCam.projectionMatrix;
            //_reflectMatrix[1, 1] = -1;
            //p *= _reflectMatrix;
            _reflectMatrixBlur[0, 0] = -1;
            p *= _reflectMatrixBlur;
            blurCam.projectionMatrix = p;*/
        }

        void FlipUCam(UCamera cam)
        {
            Matrix4x4 mat = cam.projectionMatrix;
            mat *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
            cam.projectionMatrix = mat;
        }
        bool hasBlurCam(GameCameras self)
        {
            return !(self.tk2dCam == null || self.tk2dCam.transform.GetComponentInChildren<Camera>() == null);
        }

    }
}