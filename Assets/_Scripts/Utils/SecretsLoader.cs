using UnityEngine;
using System.Collections.Generic;

namespace _Scripts.Utils
{
    [DefaultExecutionOrder(-100)]
    public class SecretsLoader : MonoBehaviour
    {
        public static Dictionary<string, string> Secrets =
            new Dictionary<string, string>();

        private void Awake() { LoadResources(); }

        private void LoadResources()
        {
            Secrets.Clear();
            TextAsset[] texts = Resources.LoadAll<TextAsset>("Secrets");

            foreach (TextAsset file in texts)
            {
                if (!Secrets.ContainsKey(file.name))
                {
                    Secrets.Add(file.name, file.text);
                }
            }
        }
    }
}
