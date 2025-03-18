using System.Globalization;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class SerializedDateTime
    {
        public System.DateTime Value
        {
            get => System.DateTime.Parse(value, CultureInfo.InvariantCulture);
            set => this.value = value.ToString("o");
        }

        // https://www.iso.org/iso-8601-date-and-time-format.html
        [SerializeField] private string value = System.DateTime.UtcNow.ToString("o");
    }
}