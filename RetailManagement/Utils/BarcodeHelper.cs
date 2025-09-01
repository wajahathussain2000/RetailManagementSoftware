using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace RetailManagement.Utils
{
    /// <summary>
    /// Helper class for generating real barcodes (Code 128 format)
    /// This implementation creates industry-standard barcodes that can be scanned by any barcode scanner
    /// </summary>
    public static class BarcodeHelper
    {
        // Code 128 character set
        private static readonly string[] Code128B = {
            " ", "!", "\"", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":", ";", "<", "=", ">", "?",
            "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "[", "\\", "]", "^", "_",
            "`", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
            "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "{", "|", "}", "~"
        };

        // Code 128 bar patterns (each character is represented by 11 bars/spaces)
        private static readonly string[] Code128Patterns = {
            "11011001100", "11001101100", "11001100110", "10010011000", "10010001100", "10001001100", "10011001000", "10011000100",
            "10001100100", "11001001000", "11001000100", "11000100100", "10110011100", "10011011100", "10011001110", "10111001100",
            "10011101100", "10011100110", "11001110010", "11001011100", "11001001110", "11011100100", "11001110100", "11101101110",
            "11101001100", "11100101100", "11100100110", "11101100100", "11100110100", "11100110010", "11011011000", "11011000110",
            "11000110110", "10100011000", "10001011000", "10001000110", "10110001000", "10001101000", "10001100010", "11010001000",
            "11000101000", "11000100010", "10110111000", "10110001110", "10001101110", "10111011000", "10111000110", "10001110110",
            "11101110110", "11010001110", "11000101110", "11011101000", "11011100010", "11011101110", "11101011000", "11101000110",
            "11100010110", "11101101000", "11101100010", "11100011010", "11101111010", "11001000010", "11110001010", "10100110000",
            "10100001100", "10010110000", "10010000110", "10000101100", "10000100110", "10110010000", "10110000100", "10011010000",
            "10011000010", "10000110100", "10000110010", "11000010010", "11001010000", "11110111010", "11000010100", "10001111010",
            "10100111100", "10010111100", "10010011110", "10111100100", "10011110100", "10011110010", "11110100100", "11110010100",
            "11110010010", "11011011110", "11011110110", "11110110110", "10101111000", "10100011110", "10001011110", "10111101000",
            "10111100010", "11110101000", "11110100010", "10111011110", "10111101110", "11101011110", "11110101110", "11010000100",
            "11010010000", "11010011100", "1100011101011"
        };

        // Start Code B, Stop pattern
        private const string StartCodeB = "11010010000";
        private const string StopPattern = "1100011101011";

        /// <summary>
        /// Generates a Code 128 barcode image for the given text
        /// </summary>
        /// <param name="text">Text to encode</param>
        /// <param name="width">Width of the barcode image</param>
        /// <param name="height">Height of the barcode image</param>
        /// <param name="showText">Whether to show the text below the barcode</param>
        /// <returns>Barcode image</returns>
        public static Bitmap GenerateCode128Barcode(string text, int width = 300, int height = 100, bool showText = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be null or empty");

            try
            {
                // Build the barcode pattern
                string barcodePattern = BuildCode128Pattern(text);
                
                // Create the barcode image
                return CreateBarcodeImage(barcodePattern, text, width, height, showText);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating barcode: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Generates a barcode specifically for pharmacy items (includes prefix)
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="prefix">Prefix for the barcode (default: "PH" for Pharmacy)</param>
        /// <param name="width">Width of the barcode image</param>
        /// <param name="height">Height of the barcode image</param>
        /// <returns>Barcode image</returns>
        public static Bitmap GeneratePharmacyBarcode(int itemId, string prefix = "PH", int width = 300, int height = 100)
        {
            // Create a standard pharmacy barcode format: PREFIX + 8-digit item ID
            string barcodeText = $"{prefix}{itemId:D8}";
            return GenerateCode128Barcode(barcodeText, width, height, true);
        }

        /// <summary>
        /// Generates EAN-13 style barcode for retail items
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="countryCode">Country code (default: 621 for Pakistan)</param>
        /// <param name="width">Width of the barcode image</param>
        /// <param name="height">Height of the barcode image</param>
        /// <returns>Barcode image</returns>
        public static Bitmap GenerateEAN13Barcode(int itemId, string countryCode = "621", int width = 300, int height = 100)
        {
            // Generate EAN-13 format: Country(3) + Company(4) + Item(5) + Check(1)
            string companyCode = "1234"; // Your company code
            string itemCode = itemId.ToString("D5");
            string baseCode = countryCode + companyCode + itemCode;
            
            // Calculate check digit
            int checkDigit = CalculateEAN13CheckDigit(baseCode);
            string ean13Code = baseCode + checkDigit.ToString();
            
            return GenerateCode128Barcode(ean13Code, width, height, true);
        }

        /// <summary>
        /// Builds the Code 128 pattern for the given text
        /// </summary>
        private static string BuildCode128Pattern(string text)
        {
            StringBuilder pattern = new StringBuilder();
            
            // Start with Code B
            pattern.Append(StartCodeB);
            
            // Calculate checksum
            int checksum = 104; // Start Code B value
            
            // Add each character
            for (int i = 0; i < text.Length; i++)
            {
                int charIndex = GetCode128Index(text[i]);
                if (charIndex == -1)
                    throw new ArgumentException($"Character '{text[i]}' is not supported in Code 128B");
                
                pattern.Append(Code128Patterns[charIndex]);
                checksum += charIndex * (i + 1);
            }
            
            // Add checksum character
            int checksumIndex = checksum % 103;
            pattern.Append(Code128Patterns[checksumIndex]);
            
            // Add stop pattern
            pattern.Append(StopPattern);
            
            return pattern.ToString();
        }

        /// <summary>
        /// Gets the index of a character in the Code 128B character set
        /// </summary>
        private static int GetCode128Index(char c)
        {
            for (int i = 0; i < Code128B.Length; i++)
            {
                if (Code128B[i].Length > 0 && Code128B[i][0] == c)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Creates the actual barcode image from the pattern
        /// </summary>
        private static Bitmap CreateBarcodeImage(string pattern, string text, int width, int height, bool showText)
        {
            int textHeight = showText ? 20 : 0;
            Bitmap bitmap = new Bitmap(width, height + textHeight);
            
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                
                // Calculate bar width
                float barWidth = (float)width / pattern.Length;
                
                // Draw bars
                float x = 0;
                for (int i = 0; i < pattern.Length; i++)
                {
                    if (pattern[i] == '1')
                    {
                        using (Brush brush = new SolidBrush(Color.Black))
                        {
                            g.FillRectangle(brush, x, 0, barWidth, height);
                        }
                    }
                    x += barWidth;
                }
                
                // Draw text if requested
                if (showText)
                {
                    using (Font font = new Font("Arial", 10, FontStyle.Regular))
                    {
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        
                        Rectangle textRect = new Rectangle(0, height, width, textHeight);
                        g.DrawString(text, font, Brushes.Black, textRect, format);
                    }
                }
            }
            
            return bitmap;
        }

        /// <summary>
        /// Calculates EAN-13 check digit
        /// </summary>
        private static int CalculateEAN13CheckDigit(string code)
        {
            int sum = 0;
            for (int i = 0; i < code.Length; i++)
            {
                int digit = int.Parse(code[i].ToString());
                sum += (i % 2 == 0) ? digit : digit * 3;
            }
            
            int remainder = sum % 10;
            return remainder == 0 ? 0 : 10 - remainder;
        }

        /// <summary>
        /// Generates a unique barcode for a new item
        /// </summary>
        /// <param name="companyId">Company ID (optional)</param>
        /// <param name="categoryCode">Category code (optional)</param>
        /// <returns>Unique barcode string</returns>
        public static string GenerateUniqueBarcode(int? companyId = null, string categoryCode = null)
        {
            // Generate timestamp-based unique code
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            // Create barcode with company and category if provided
            StringBuilder barcode = new StringBuilder();
            
            if (companyId.HasValue)
                barcode.Append($"C{companyId.Value:D2}");
            
            if (!string.IsNullOrEmpty(categoryCode))
                barcode.Append($"T{categoryCode.Substring(0, Math.Min(2, categoryCode.Length)).ToUpper()}");
            
            // Add timestamp (last 8 digits for uniqueness)
            barcode.Append((timestamp % 100000000).ToString("D8"));
            
            return barcode.ToString();
        }

        /// <summary>
        /// Validates if a string can be encoded as Code 128
        /// </summary>
        /// <param name="text">Text to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidCode128Text(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            
            foreach (char c in text)
            {
                if (GetCode128Index(c) == -1)
                    return false;
            }
            
            return true;
        }
    }
}
