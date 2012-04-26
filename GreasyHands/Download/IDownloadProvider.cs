using GreasyHands.Search;

namespace GreasyHands.Download
{
    public interface IDownloadProvider
    {
        bool QueueDownload(Result result);
    }
}
