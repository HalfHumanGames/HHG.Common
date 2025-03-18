namespace HHG.Common.Runtime
{
    public class OptionsDropdownAttribute : OptionsAttribute
    {
        public string OptionsMemberName => optionsMemberName;

        private string optionsMemberName;

        public OptionsDropdownAttribute(string optionsFieldName)
        {
            this.optionsMemberName = optionsFieldName;
        }
    }
}