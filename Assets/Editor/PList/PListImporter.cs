using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lite.Editor.PList
{
    public class PListImporter : MonoBehaviour
    {
        [MenuItem("Lite/Import PList")]
        public static void ImportPlistMenu()
        {
            var SelectObj = Selection.activeObject;
            var SelectionPath = AssetDatabase.GetAssetPath(SelectObj);
            if (SelectObj != null && SelectionPath.EndsWith(".plist"))
            {
                ImportPList(SelectObj);
                return;
            }
            else
            {
                PListOptionWindow.ShowWindow();
            }
        }

        public static void ImportPList(UnityEngine.Object Obj)
        {
            var SelectionPath = AssetDatabase.GetAssetPath(Obj);
            if (!SelectionPath.EndsWith(".plist"))
            {
                EditorUtility.DisplayDialog("Error", "Please select a plist file!", "OK");
                return;
            }

            var Parser = new PListParser();
            if (!Parser.Parse(SelectionPath))
            {
                EditorUtility.DisplayDialog("Error", "PList file format error!", "OK");
                return;
            }

            var TexPath = ($"{Path.GetDirectoryName(SelectionPath)}/{Parser.RealTextureName}").Replace('\\', '/');
            var Tex = (AssetDatabase.LoadAssetAtPath(TexPath, typeof(Texture2D)) as Texture2D);
            if (Tex == null)
            {
                EditorUtility.DisplayDialog("Error", "PList texture can't find!", "OK");
                return;
            }

            var Importer = AssetImporter.GetAtPath(TexPath) as TextureImporter;
            if (Importer == null || Importer.textureType != TextureImporterType.Sprite)
            {
                EditorUtility.DisplayDialog("Error", "PList texture type must be sprite!", "OK");
                return;
            }

            var Frames = Parser.Frames;
            var SheetMetas = new SpriteMetaData[Frames.Count];
            var Index = 0;

            foreach (var Frame in Frames)
            {
                SheetMetas[Index].alignment = 0;
                SheetMetas[Index].border = new Vector4(0, 0, 0, 0);
                SheetMetas[Index].name = Frame.Key;
                SheetMetas[Index].pivot = new Vector2(0.5f, 0.5f);
                SheetMetas[Index].rect = new Rect(
                    Frame.Value.Frame.x,
                    Parser.TextureSize.y - Frame.Value.Frame.y - Frame.Value.Frame.height,
                    Frame.Value.Frame.width,
                    Frame.Value.Frame.height);//这里原点在左下角，y相反
                Index++;
            }

            Importer.spritesheet = SheetMetas;

            Importer.textureType = TextureImporterType.Sprite;
            Importer.spriteImportMode = SpriteImportMode.Multiple;
            AssetDatabase.ImportAsset(TexPath, ImportAssetOptions.ForceUpdate);

            Debug.LogWarning("Parser Over :" + TexPath);
        }
    }
}