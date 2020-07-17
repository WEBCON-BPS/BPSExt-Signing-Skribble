using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.Skribble.CustomActions.Models.Configuration
{
    public class InputAttConfig : PluginConfiguration
    {
        [ConfigEditableEnum(DisplayName = "Selection mode", Description = "Category/ SQL query", DefaultValue = 0)]
        public InputType InputAttType { get; set; }

        [ConfigEditableEnum(DisplayName = "Category mode", Description = "Select ‘None’ for files not associated with any category or ‘All’ for attachment from the element.", DefaultValue = 0)]
        public CategoryType CatType { get; set; }

        [ConfigEditableText(DisplayName = "Category ID", Description = "Select the attachment category ID to be signed.")]
        public int InputCategoryId { get; set; }

        [ConfigEditableText(DisplayName = "Regex expression", Description = "Regular expression can be used as an additional filter for attachments from the selected category", DefaultText = ".*[.]pdf")]
        public string AttRegularExpression { get; set; }

        [ConfigEditableText(DisplayName = "SQL query", Description = @"Query should return a list of attachments' IDs from WFDataAttachmets table.
Example: Select [ATT_ID] from [WFDataAttachmets] Where [ATT_Name] = 'agreement.pdf'", Multiline = true, TagEvaluationMode = EvaluationMode.SQL)]
        public string AttQuery { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Copy attachment ID to field", Description = "Specify a technical field on the form where source attachment ID will be saved")]
        public int AttTechnicalFieldID { get; set; }
    }

    public enum InputType
    {
        Category,
        SQL
    }

    public enum CategoryType
    {
        ID,
        All,
        None
    }

}
