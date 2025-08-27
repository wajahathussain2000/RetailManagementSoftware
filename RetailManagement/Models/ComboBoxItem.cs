using System;

namespace RetailManagement.Models
{
    /// <summary>
    /// Helper class for ComboBox items with consistent property names
    /// </summary>
    public class ComboBoxItem
    {
        public object Value { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
