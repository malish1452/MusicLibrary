using System.ComponentModel;

namespace DataHelper.Enums
{
    public enum LogicTypeEnum
    {
        [Description("Обновление списоков треков")] PlaylistServiceLogic,

        [Description("Обновление треков")] TrackProcess
    }
}
