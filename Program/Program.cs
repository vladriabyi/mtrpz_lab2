using System;
using System.IO;

class Program
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
            ChangeText();
            CheckIncluded(text);
            AddHTMLStructure(ref text);
            CreateHTML(text, path, name, format);
            Console.WriteLine("Файл створено!");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error: " + ex);
            Console.ReadKey();
        }
    }

    static string ParseCommandLine(string[] args)
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

    static void CreateHTML(string text, string path, string name, string format)
    {
        string filePath = path + "\\" + name + "." + format;
        File.WriteAllText(filePath, text);
    }

    static void AskForFile(ref string text, ref string path, ref string name)
    {
        Console.WriteLine("Зазначте шлях до текстового файлу\nНаприклад: \"C:\\Documents\\file.md\"");
        string filePath = Console.ReadLine();
        if (Path.GetExtension(filePath) == ".md")
        {
            if (File.Exists(filePath))
            {
                text = File.ReadAllText(filePath);
                path = Path.GetDirectoryName(filePath) + "\\";
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

    static void ChangeText()
    {
        for (int i = 0; i < text.Length; i++)
        {
            CheckItalic(ref text, i);
            CheckBold(ref text, i);
            CheckPref(ref text, ref i);
            CheckMono(ref text, i);
        }
    }

    static void CheckItalic(ref string text, int i)
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

    static void CloseItalic(ref string text, int i)
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

    static void CheckBold(ref string text, int i)
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

    static void CloseBold(ref string text, int i)
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

    static void CheckMono(ref string text, int i)
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

    static void CloseMono(ref string text, int i)
    {
        for (int j = i + 1; j < text.Length; j++)
        {
            if (text[j] == '`')
            {
                if (j > 0)
                {
                    if (char.IsLetterOrDigit(text[j - 1]))
                    {
                        text = text.Remove(j, 1);
                        text = text.Insert(j, "</tt>");
                        break;
                    }
                    else if (char.IsPunctuation(text[j - 1]) || text[j - 1] == '`' || text[j - 1] == '>')
                    {
                        text = text.Remove(j, 1);
                        text = text.Insert(j, "</tt>");
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

    static void CheckPref(ref string text, ref int i)
    {
        if (text[i] == '`')
        {
            if (i < text.Length - 3 && text[i + 1] == '`' && text[i + 2] == '`')
            {
                text = text.Remove(i, 3);
                text = text.Insert(i, "<pre>");
                ClosePref(ref text, ref i);
            }
        }
    }

    static void ClosePref(ref string text, ref int i)
    {
        for (int j = i + 1; j < text.Length; j++)
        {
            if (text[j] == '`')
            {
                if (j > 2 && text[j - 1] == '`' && text[j - 2] == '`')
                {
                    text = text.Remove(j - 2, 3);
                    text = text.Insert(j - 2, "</pre>");
                    i = j;
                    break;
                }
            }
            else if (j == text.Length - 1)
            {
                throw new Exception("Незакриті розділові знаки");
            }
        }
    }

    static void CheckParag(ref string text)
    {
        text = "<p>" + text;
        CloseParag(ref text, 0);
    }

    static void CloseParag(ref string text, int i)
    {
        for (int j = i; j < text.Length; j++)
        {
            if (text[j] == '\n')
            {
                if (j < text.Length - 3 && text[j + 2] == '\n')
                {
                    text = text.Remove(j - 1, 4);
                    text = text.Insert(j - 1, "</p><p>");
                    CloseParag(ref text, j);
                    break;
                }
                else if (j == text.Length - 3 && text[j + 2] == '\n')
                {
                    text = text.Remove(j - 1, 4);
                    text = text + "</p>";
                    break;
                }
                else if (j == text.Length - 1)
                {
                    text = text.Remove(j - 1, 2);
                    text = text + "</p>";
                    break;
                }
            }
            else if (j == text.Length - 1)
            {
                text = text + "</p>";
                break;
            }
        }
    }

    static void CheckIncluded(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (i < text.Length - 2 && text.Substring(i, 3) == "<i>")
            {
                CheckIncluded_I(text, i);
            }
            else if (i < text.Length - 2 && text.Substring(i, 3) == "<b>")
            {
                CheckIncluded_B(text, i);
            }
            else if (i < text.Length - 3 && text.Substring(i, 4) == "<tt>")
            {
                CheckIncluded_TT(text, i);
            }
        }
    }

    static void CheckIncluded_I(string text, int i)
    {
        for (int j = i + 2; j < text.Length; j++)
        {
            if (j < text.Length - 3 && text.Substring(j, 4) == "</i>")
            {
                break;
            }
            else if (j < text.Length - 2 && text.Substring(j, 3) == "<b>")
            {
                throw new Exception("Застосовано пунктуацію");
            }
            else if (j < text.Length - 3 && text.Substring(j, 4) == "<tt>")
            {
                throw new Exception("Застосовано пунктуацію");
            }
        }
    }

    static void CheckIncluded_B(string text, int i)
    {
        for (int j = i + 2; j < text.Length; j++)
        {
            if (j < text.Length - 3 && text.Substring(j, 4) == "</b>")
            {
                break;
            }
            else if (j < text.Length - 2 && text.Substring(j, 3) == "<i>")
            {
                throw new Exception("Застосовано пунктуацію");
            }
            else if (j < text.Length - 3 && text.Substring(j, 4) == "<tt>")
            {
                throw new Exception("Застосовано пунктуацію");
            }
        }
    }

    static void CheckIncluded_TT(string text, int i)
    {
        for (int j = i + 2; j < text.Length; j++)
        {
            if (j < text.Length - 4 && text.Substring(j, 5) == "</tt>")
            {
                break;
            }
            else if (j < text.Length - 2 && text.Substring(j, 3) == "<b>")
            {
                throw new Exception("Застосовано пунктуацію");
            }
            else if (j < text.Length - 4 && text.Substring(j, 3) == "<i>")
            {
                throw new Exception("Застосовано пунктуацію");
            }
        }
    }

    static void CreateHTML(string text, string path, string name)
    {
        string filePath = path + "\\" + name + ".html";
        File.WriteAllText(filePath, text);
    }

    static void AddHTMLStructure(ref string text)
    {
        text =
@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>
</head>
<body>
" + text +
@"</body>
</html>";
    }


}
