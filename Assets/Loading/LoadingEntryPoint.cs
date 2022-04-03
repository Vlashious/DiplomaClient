using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Loading
{
    public sealed class LoadingEntryPoint : LifetimeScope
    {
        [SerializeField]
        private Button _startButton;
        [SerializeField]
        private Slider _progressBar;
        [SerializeField]
        private TMP_Text _progressBarText;

        private SceneInstance _sceneToLoad;

        protected override void Configure(IContainerBuilder builder)
        {
            _startButton.onClick.AddListener(() => StartCoroutine(OnStartButtonClicked()));
        }

        private IEnumerator OnStartButtonClicked()
        {
            Addressables.ClearDependencyCacheAsync("default");
            var downloadSize = Addressables.GetDownloadSizeAsync("default");

            yield return downloadSize;

            if (downloadSize.Result > 0)
            {
                AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync("default");

                while (handle.Status is AsyncOperationStatus.None)
                {
                    var downloadProgress = handle.GetDownloadStatus();
                    var progress = downloadProgress.Percent;
                    _progressBarText.SetText($"{100 * progress:0.##}%");
                    _progressBar.value = progress;
                    yield return null;
                }

                Addressables.Release(handle);
            }

            var scene = Addressables.LoadSceneAsync("1.World");
            yield return scene;
            _sceneToLoad = scene.Result;
        }
    }
}