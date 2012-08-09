namespace CDWKS.Model.EF.FileQueue
{
    public class Enums
    {
        public enum IndexType {Full,NewOnly,NewAndVersionUpdate, UpdateTree};
        public enum TaskAction {Index, Wait, Stop}
        public enum IndexStatus {Succeeded, DataSyncFailed, ContentUploadFailed, Queued, DataSynced, ContentUploaded}
    }
}
