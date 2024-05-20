using System;
using System.IO;

namespace Program
{
    public class Program
    {
        static string text = @"";
        static string path = "", name = "";

        static void Main(string[] args)
        {
            try
            {
                string format = ParseCommandLine(args);
                AskForFile(ref text, ref path, ref name);
                CheckParag(ref text);
                text = ChangeText(text);
                CheckIncluded(text);
                AddHTMLStructure(ref text);
                CreateHTML(text, path, name);
                Console.WriteLine("Файл створено!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                Console.ReadKey();
            }
        }

        public static string ParseCommandLine(string[] args)
        {
            string format = "html"; // Default format is HTML
            foreach (string arg in args)
            {
                if (arg.StartsWith("--format="))
                {
                    format = arg.Substring("--format=".Length).ToLower(); // Extract the format value
                    break;
                }
            }
            return format;
        }

        public static void CreateHTML(string text, string path, string name)
        {
            string filePath = Path.Combine(path, name + ".html");
            File.WriteAllText(filePath, text);
        }

        public static void CreateHTML(string text, string path, string name, string format)
        {
            if (format != "html")
            {
                throw new ArgumentException("Unsupported format");
            }
            string filePath = Path.Combine(path, name + ".html");
            File.WriteAllText(filePath, text);
        }

        public static void AskForFile(ref string text, ref string path, ref string name)
        {
            Console.WriteLine("Зазначте шлях до текстового файлу\nНаприклад: \"C:\\Documents\\file.md\"");
            string filePath = Console.ReadLine();
            if (Path.GetExtension(filePath) == ".md")
            {
                if (File.Exists(filePath))
                {
                    text = File.ReadAllText(filePath);
                    path = Path.GetDirectoryName(filePath);
                    name = Path.GetFileNameWithoutExtension(filePath);
                }
                else
                {
                    Console.WriteLine("Файл не знайдено, спробуйте ще раз.");
                    AskForFile(ref text, ref path, ref name);
                }
            }
            else
            {
                Console.WriteLine("Слід використати файл з розширенням .md. Спробуйте ще раз.");
                AskForFile(ref text, ref path, ref name);
            }
        }

        public static string ChangeText(string text)
        {
            string newText = text; // Створюємо нову змінну для зміненого тексту

            for (int i = 0; i < newText.Length; i++)
            {
                CheckItalic(ref newText, i);
                CheckBold(ref newText, i);
                CheckPref(ref newText, ref i);
                CheckMono(ref newText, i);
            }

            return newText; // Повертаємо змінений текст
        }

        public static void CheckItalic(ref string text, int i)
        {
            if (text[i] == '_')
            {
                if (i == 0 || (i > 0 && !char.IsLetterOrDigit(text[i - 1])))
                {
                    if (i < text.Length - 1)
                    {
                        if (char.IsLetterOrDigit(text[i + 1]))
                        {
                            text = text.Remove(i, 1);
                            text = text.Insert(i, "<i>");
                            CloseItalic(ref text, i);
                        }
                        else if (char.IsPunctuation(text[i + 1]) || text[i + 1] == '`')
                        {
                            text = text.Remove(i, 1);
                            text = text.Insert(i, "<i>");
                            CloseItalic(ref text, i);
                        }
                    }
                }
            }
        }

        public static void CloseItalic(ref string text, int i)
        {
            for (int j = i + 1; j < text.Length; j++)
            {
                if (text[j] == '_')
                {
                    if (j == text.Length - 1 || (j < text.Length - 1 && !char.IsLetterOrDigit(text[j + 1])))
                    {
                        if (j > 0)
                        {
                            if (char.IsLetterOrDigit(text[j - 1]))
                            {
                                text = text.Remove(j, 1);
                                text = text.Insert(j, "</i>");
                                break;
                            }
                            else if (char.IsPunctuation(text[j - 1]) || text[j - 1] == '`' || text[j - 1] == '>')
                            {
                                text = text.Remove(j, 1);
                                text = text.Insert(j, "</i>");
                                break;
                            }
                        }
                    }
                }
                else if (j == text.Length - 1)
                {
                    throw new Exception("Незакриті розділові знаки");
                }
            }
        }

        public static void CheckBold(ref string text, int i)
        {
            if (text[i] == '*')
            {
                if (i < text.Length - 2 && text[i + 1] == '*')
                {
                    if (char.IsLetterOrDigit(text[i + 2]))
                    {
                        text = text.Remove(i, 2);
                        text = text.Insert(i, "<b>");
                        CloseBold(ref text, i);
                    }
                    else if (char.IsPunctuation(text[i + 2]) || text[i + 2] == '`')
                    {
                        text = text.Remove(i, 2);
                        text = text.Insert(i, "<b>");
                        CloseBold(ref text, i);
                    }
                }
            }
        }

        public static void CloseBold(ref string text, int i)
        {
            for (int j = i + 1; j < text.Length; j++)
            {
                if (text[j] == '*')
                {
                    if (j > 1 && text[j - 1] == '*')
                    {
                        if (char.IsLetterOrDigit(text[j - 2]))
                        {
                            text = text.Remove(j - 1, 2);
                            text = text.Insert(j - 1, "</b>");
                            break;
                        }
                        else if (char.IsPunctuation(text[j - 2]) || text[j - 2] == '`' || text[j - 2] == '>')
                        {
                            text = text.Remove(j - 1, 2);
                            text = text.Insert(j - 1, "</b>");
                            break;
                        }
                    }
                }
                else if (j == text.Length - 1)
                {
                    throw new Exception("Незакриті розділові знаки");
                }
            }
        }

        public static void CheckMono(ref string text, int i)
        {
            if (text[i] == '`')
            {
                if (i < text.Length - 1)
                {
                    if (char.IsLetterOrDigit(text[i + 1]))
                    {
                        text = text.Remove(i, 1);
                        text = text.Insert(i, "<tt>");
                        CloseMono(ref text, i);
                    }
                    else if (char.IsPunctuation(text[i + 1]) || text[i + 1] == '`')
                    {
                        text = text.Remove(i, 1);
                        text = text.Insert(i, "<tt>");
                        CloseMono(ref text, i);
                    }
                }
            }
        }

        public static void CloseMono(ref string text, int i)
        {
            for (int j = i + 1; j < text.Length; j++)
            {
                if (text[j] == '`')
                {
                    text = text.Remove(j, 1);
                    text = text.Insert(j, "</tt>");
                    break;
                }
                else if (j == text.Length - 1)
                {
                    throw new Exception("Незакриті розділові знаки");
                }
            }
        }

        public static void CheckPref(ref string text, ref int i)
        {
            if (text[i] == '`')
            {
                if (i < text.Length - 2 && text[i + 1] == '`' && text[i + 2] == '`')
                {
                    text = text.Remove(i, 3);
                    text = text.Insert(i, "<pre>");
                    ClosePref(ref text, i);
                }
            }
        }

        public static void ClosePref(ref string text, int i)
        {
            for (int j = i + 1; j < text.Length; j++)
            {
                if (text[j] == '`')
                {
                    if (j < text.Length - 2 && text[j + 1] == '`' && text[j + 2] == '`')
                    {
                        text = text.Remove(j, 3);
                        text = text.Insert(j, "</pre>");
                        break;
                    }
                }
                else if (j == text.Length - 1)
                {
                    throw new Exception("Незакриті розділові знаки");
                }
            }
        }

        public static void CheckParag(ref string text)
        {
            string[] lines = text.Split('\n');
            text = "";

            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    text += $"<p>{line}</p>";
                }
            }
        }

        public static void AddHTMLStructure(ref string text)
        {
            text = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>
</head>
<body>
" + text + @"
</body>
</html>";
        }

        public static void CheckIncluded(string text)
        {
            if (text.Contains("<tt>") && text.Contains("</tt>") &&
                text.Contains("<b>") && text.Contains("</b>") &&
                text.Contains("<i>") && text.Contains("</i>") &&
                text.Contains("<pre>") && text.Contains("</pre>") &&
                text.Contains("<p>") && text.Contains("</p>"))
            {
                Console.WriteLine("All tags are included.");
            }
            else
            {
                Console.WriteLine("Not all tags are included.");
            }
        }
    }
}
