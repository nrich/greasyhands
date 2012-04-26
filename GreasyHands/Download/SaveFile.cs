using GreasyHands.Search;

namespace GreasyHands.Download
{
    class SaveFile : IDownloadProvider
    {
        public string Path { get; set; }

        public bool QueueDownload(Result result)
        {
            throw new System.NotImplementedException();
        }
    }
}