using Xamarin.Forms;

namespace NimbleBlocks.Helpers
{
    public static class AccessibilityHelper
    {
        public static void SetAccessibilityProperties(Element element, string automationId, string name, string helpText = null, bool isInAccessibleTree = true)
        {
            if (element == null) return;

            element.AutomationId = automationId;
            AutomationProperties.SetName(element, name);
            AutomationProperties.SetIsInAccessibleTree(element, isInAccessibleTree);
            
            if (!string.IsNullOrEmpty(helpText))
            {
                AutomationProperties.SetHelpText(element, helpText);
            }
        }

        public static void SetButtonAccessibility(Button button, string automationId, string name, string helpText = null)
        {
            SetAccessibilityProperties(button, automationId, name, helpText);
        }

        public static void SetImageAccessibility(Image image, string automationId, string name, string helpText = null)
        {
            SetAccessibilityProperties(image, automationId, name, helpText);
        }

        public static void SetLabelAccessibility(Label label, string automationId, string name, string helpText = null)
        {
            SetAccessibilityProperties(label, automationId, name, helpText);
        }

        public static void SetSliderAccessibility(Slider slider, string automationId, string name, string helpText = null)
        {
            SetAccessibilityProperties(slider, automationId, name, helpText);
        }

        public static void SetSwitchAccessibility(Switch switchControl, string automationId, string name, string helpText = null)
        {
            SetAccessibilityProperties(switchControl, automationId, name, helpText);
        }
    }
}
