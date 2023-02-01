using Undebugger.Model.Status;
using UnityEngine;

namespace Undebugger.Model.Status.Builtin
{
    internal class DeviceInfoStatusSegment : StaticStatusSegmentDriver
    {
        private const string LabelColor = "#A2A2A2";

        public static DeviceInfoStatusSegment Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DeviceInfoStatusSegment();
                }

                return instance;
            }
        }

        private static DeviceInfoStatusSegment instance;

        public override int Priority
        { get { return -9003; } }

        public DeviceInfoStatusSegment()
            : base("device_info", "Device", GenerateString())
        { }

        private static string GenerateString()
        {
            return
$@"<color={LabelColor}>Name:</color> {SystemInfo.deviceName}
<color={LabelColor}>Model:</color> {SystemInfo.deviceModel}
<color={LabelColor}>UID:</color> {SystemInfo.deviceUniqueIdentifier}
<color={LabelColor}>Type:</color> {SystemInfo.deviceType}
<color={LabelColor}>Supports Location Service:</color> {SystemInfo.supportsLocationService}
<color={LabelColor}>Supports Accelerometer:</color> {SystemInfo.supportsAccelerometer}
<color={LabelColor}>Supports Gyroscope:</color> {SystemInfo.supportsGyroscope}
<color={LabelColor}>Supports Vibration:</color> {SystemInfo.supportsVibration}";
        }
    }
}
