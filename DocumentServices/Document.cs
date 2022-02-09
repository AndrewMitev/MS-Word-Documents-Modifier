using System.Collections.Generic;
using System.Text;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace DocumentServices
{
    public static class Document
    {
        public static void AddHeadings(string filePath, bool isNumberedDictionary, string fontFamily = "", double fontSize = 0.0d)
        {
            using (var document = DocX.Load(filePath))
            {
                var paragraphs = document.Paragraphs;
                StringBuilder lemmaBuilder = new StringBuilder();

                foreach (var paragraph in paragraphs)
                {
                    foreach (var word in paragraph.MagicText)
                    {
                        bool isDefaultLemma = word.text != string.Empty && word.formatting.Bold == true;

                        bool isSpecifiedLemma = fontFamily != string.Empty || fontSize != 0.0d;

                        bool isSpecificationMatched = word.formatting.FontFamily.Name.ToLower().Equals(fontFamily.ToLower()) 
                            && word.formatting.Size == fontSize;

                        if (isDefaultLemma && (isSpecifiedLemma ? isSpecificationMatched : true))
                        {
                            int currentIndex = paragraph.MagicText.IndexOf(word);
                            string paragraphText = isAdditionalAssemblyDone(paragraph.MagicText, lemmaBuilder, ++currentIndex) ?
                                word.text + lemmaBuilder.ToString() :
                                word.text;

                            paragraphText = (paragraphText.EndsWith(".") || paragraphText.EndsWith(",")) ?
                                paragraphText.Substring(0, paragraphText.Length - 1) : paragraphText;

                            paragraphText = isNumberedDictionary ?
                                paragraphText :
                                PrePendText(paragraph.MagicText, paragraph.MagicText.IndexOf(word)) + paragraphText;

                            var p = paragraph.InsertParagraphBeforeSelf(paragraphText);
                            p.StyleId = HeadingType.Heading1.ToString();
                            p.FontSize(16.0);
                            p.Bold(false);
                            p.SpacingBefore(12.0d);

                            lemmaBuilder.Clear();
                            break;
                        }
                    }
                }

                document.SaveAs(GetOuputFilename(filePath));
            }
        }

        public static void RemoveHeadings(string filePath) 
        {
            using (var document = DocX.Load(filePath))
            {
                foreach (var pars2 in document.Paragraphs)
                {
                    if (pars2.StyleId == HeadingType.Heading1.ToString() || pars2.StyleId == "Kop1")
                    {
                        document.RemoveParagraph(pars2);
                    }
                }

                document.SaveAs(GetOuputFilename(filePath));
            }
        }

        private static string GetOuputFilename(string originalFilePath)
        {
            return originalFilePath.Insert(originalFilePath.LastIndexOf('.'), "_output");
        }

        private static string PrePendText(List<FormattedText> paragraph, int currentIndex)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < currentIndex; i++)
            {
                sb.Append(paragraph[i].text);
            }

            return sb.ToString();
        }

        private static bool isAdditionalAssemblyDone(List<FormattedText> paragraph, StringBuilder lemmaBuilder, int index)
        {
            FormattedText word = paragraph[index];

            if (word.text != string.Empty && word.text != "," && word.text != ", ")
            {
                if (word.formatting.Bold == true)
                {
                    lemmaBuilder.Append(word.text);
                    return true;
                }

                return false;
            }

            lemmaBuilder.Append(word.text);
            return isAdditionalAssemblyDone(paragraph, lemmaBuilder, ++index);
        }
    }
}
