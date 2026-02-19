using System;
using System.Collections.Generic;

internal class Program
{
    // Wortlisten für beide Modi
    static string[] normalWords =
    {
        "Taschenlampe",
        "Programmierung",
        "Kaffeemaschine",
        "Schreibtisch",
        "Bildschirm",
        "Wasserflasche"
    };

    static string[] hardcoreWords =
    {
        "Donaudampfschifffahrtsgesellschaftskapitän",
        "Grundstücksverkehrsgenehmigungszuständigkeitsübertragungsverordnung",
        "Donaudampfschifffahrtselektrizitätenhauptbetriebswerkbauunterbeamtengesellschaft",
        "Rinderkennzeichnungsfleischetikettierungsüberwachungsaufgabenübertragungsgesetz"
    };

    static void Main(string[] args)
    {
        var rng = new Random();

        // Schwierigkeitseinstellungen
        bool difficulty = false;
        bool hardcore = false;
        int maxWrong = 6;

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Hangman ===");
            Console.WriteLine("1) Neues Spiel");
            Console.WriteLine("2) Schwierigkeit");
            Console.WriteLine("3) Regeln");
            Console.WriteLine("4) Beenden");
            Console.Write("Auswahl: ");

            string? choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                if (!difficulty)
                {
                    hardcore = false;
                    maxWrong = 6;
                }

                string[] currentWords = hardcore ? hardcoreWords : normalWords;
                PlayOneGame(currentWords, rng, maxWrong);

                if (!AskYesNo("Weiter ? (j = Menu / n = Beenden) "))
                    break;
            }
            else if (choice == "2")
            {
                SelectDifficulty(ref difficulty, ref hardcore, ref maxWrong);
            }
            else if (choice == "3")
            {
                ShowRules();
            }
            else if (choice == "4")
            {
                break;
            }
            else
            {
                Console.WriteLine("Ungültige Auswahl.");
            }
        }
    }
    static void SelectDifficulty(ref bool difficulty, ref bool hardcore, ref int maxWrong) // ref-Paramereter um die Werte in Main zu ändern
    {
        Console.WriteLine();
        Console.WriteLine("=== Schwierigkeit wählen ===");
        Console.WriteLine("1) Normal (6 Fehlversuche, einfache Wörter)");
        Console.WriteLine("2) Hardcore (wählbar: 6/8/10 Fehlversuche, schwere Wörter)");
        Console.Write("Auswahl: ");

        string? choice = Console.ReadLine()?.Trim();

        if (choice == "1")
        {
            difficulty = true;
            hardcore = false;
            maxWrong = 6;
            Console.WriteLine("Normal-Modus aktiviert.");
        }
        else if (choice == "2")
        {
            Console.Write("Fehlversuche für Hardcore-Modus (6/8/10): ");
            string? input = Console.ReadLine()?.Trim();

            if (input == "6" || input == "8" || input == "10")
            {
                difficulty = true;
                hardcore = true;
                maxWrong = int.Parse(input);
                Console.WriteLine($"Hardcore-Modus aktiviert mit {maxWrong} Fehlversuchen.");
            }
            else
            {
                Console.WriteLine("Ungültige Eingabe. Nur 6, 8 oder 10 erlaubt.");
            }
        }
        else
        {
            Console.WriteLine("Ungültige Auswahl.");
        }
    }

    static void PlayOneGame(string[] words, Random rng, int maxWrong)
    {
        string wort = words[rng.Next(words.Length)];
        string secret = wort.ToLowerInvariant();

        char[] progress = new char[secret.Length];
        for (int i = 0; i < progress.Length; i++) progress[i] = '_';

        int wrong = 0;
        bool solvedByWord = false;
        var guessed = new HashSet<char>();

        while (wrong < maxWrong && !solvedByWord && !IsSolved(progress))
        {
            Console.WriteLine();
            PrintProgress(progress);
            Console.WriteLine($"Fehlversuche: {wrong}/{maxWrong}");
            Console.Write("Rate einen Buchstaben oder das ganze Wort: ");

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Ungültige Eingabe (leer).");
                continue;
            }

            string guessText = input.Trim().ToLowerInvariant();

            // Wortversuch
            if (guessText.Length > 1)
            {
                if (guessText == secret)
                {
                    for (int i = 0; i < secret.Length; i++)
                        progress[i] = secret[i];

                    solvedByWord = true;
                    Console.WriteLine("Richtiges Wort.");
                }
                else
                {
                    wrong++;
                    Console.WriteLine("Falsches Wort.");
                }
                continue;
            }

            // Buchstabe
            char guess = guessText[0];

            if (!char.IsLetter(guess))
            {
                Console.WriteLine("Bitte einen Buchstaben eingeben.");
                continue;
            }

            if (!guessed.Add(guess))
            {
                Console.WriteLine("Diesen Buchstaben hast du schon versucht.");
                continue;
            }

            bool hit = false;
            for (int i = 0; i < secret.Length; i++)
            {
                if (secret[i] == guess)
                {
                    progress[i] = guess;
                    hit = true;
                }
            }

            if (!hit)
            {
                wrong++;
                Console.WriteLine("Falsch.");
            }
            else
            {
                Console.WriteLine("Richtig.");
            }
        }

        Console.WriteLine();
        PrintProgress(progress);

        if (IsSolved(progress))
            Console.WriteLine($"Gewonnen. Wort: {wort}");
        else
            Console.WriteLine($"Verloren. Das Wort war: {wort}");
    }

    static void ShowRules()
    {
        Console.WriteLine();
        Console.WriteLine("Regeln:");
        Console.WriteLine("- Du errätst ein unbekanntes Wort.");
        Console.WriteLine("- Gib einen Buchstaben oder das ganze Wort ein.");
        Console.WriteLine("- Falsche Eingaben erhöhen die Fehlversuche.");
        Console.WriteLine("- Bei zu vielen Fehlversuchen verlierst du.");
    }

    static bool AskYesNo(string prompt)
    {
        Console.Write(prompt);
        string? answer = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(answer)) return false;

        answer = answer.Trim().ToLowerInvariant();
        return answer == "j" || answer == "ja" || answer == "y" || answer == "yes" || answer == "da";
    }

    static bool IsSolved(char[] progress)
    {
        for (int i = 0; i < progress.Length; i++)
            if (progress[i] == '_') return false;
        return true;
    }

    static void PrintProgress(char[] progress)
    {
        for (int i = 0; i < progress.Length; i++)
        {
            Console.Write(progress[i]);
            if (i < progress.Length - 1) Console.Write(' ');
        }
        Console.WriteLine();
    }
}
