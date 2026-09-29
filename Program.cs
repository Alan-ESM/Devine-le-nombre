using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

public class Program
{
    const string FICHIER_CLASSEMENT = "score.csv";
    const ConsoleColor COULEUR_BRONZE = ConsoleColor.DarkYellow;

    public struct Difficulte { public int Max; public int Vies; public int Indices; public string Nom; }
    public class ScoreboardEntry { public string Nom; public double TempsMeilleur; public int ViesRestantes; public int MaxAtteint; }

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Devine le Nombre BY G5 🎮";
        AfficherBarreChargement();
        Console.Beep(800, 200);
        Console.WriteLine("=== Bienvenue dans le jeu 'Devine le Nombre' ! 🎯 ===\n");
        string nomJoueur = DemanderNomJoueur();
        Console.Beep(800, 200);
        Console.WriteLine($"\nBienvenue, {nomJoueur} ! Prépare-toi à jouer. 😄");

        bool continuerJeu = true;
        while (continuerJeu)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n--- Menu Principal 🏠 ---");
            Console.WriteLine("1. 🎯 Devine le nombre");
            Console.WriteLine("2. 🤖 Fais deviner le nombre");
            Console.WriteLine("3. 📊 Afficher le classement");
            Console.WriteLine("4. 🚪 Quitter");
            Console.ResetColor();
            Console.Write("Choisis une option (1-4) : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1": JoueurDevine(nomJoueur); break;
                case "2": MachineDevine(nomJoueur); break;
                case "3": AfficherClassement(); break;
                case "4": continuerJeu = false; Console.WriteLine($"\nMerci d'avoir joué, {nomJoueur} ! À bientôt. 👋"); break;
                default: Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("❌ Option invalide."); Console.ResetColor(); break;
            }
        }
    }

    public static void AfficherBarreChargement()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("🔄 Initialisation du jeu...\n");

        int largeurBarre = 50;
        for (int i = 0; i <= largeurBarre; i++)
        {
            double pourcentage = (double)i / largeurBarre * 100;
            string barre = "[" + new string('█', i) + new string(' ', largeurBarre - i) + "]";

            Console.Write($"\r{barre} {pourcentage:F0}%");
            System.Threading.Thread.Sleep(50);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n\n✅ Jeu chargé avec succès !");
        System.Threading.Thread.Sleep(1000);
        Console.Clear();
        Console.ResetColor();
    }

    public static string DemanderNomJoueur()
    {
        string nom; bool nomValide = false;
        do
        {
            Console.Write("🎭 Veuillez entrer votre nom : "); nom = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nom)) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("❌ Le nom ne peut pas être vide."); Console.ResetColor(); }
            else if (Regex.IsMatch(nom, @"\d")) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("❌ Votre nom ne doit pas contenir de chiffres."); Console.ResetColor(); }
            else { nom = VerifierJoueurExistant(nom); nomValide = true; }
        } while (!nomValide);
        return nom;
    }

    public static string VerifierJoueurExistant(string nom)
    {
        List<ScoreboardEntry> classement = ChargerClassement();
        string nomNormalise = new string(nom.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower();
        var joueurSimilaire = classement.FirstOrDefault(e => new string(e.Nom.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower() == nomNormalise);

        if (joueurSimilaire != null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n👀 Je vois que '{joueurSimilaire.Nom}' a déjà joué ici...");
            Console.WriteLine("🤔 Est-ce la même personne ?");
            Console.ResetColor();
            bool reponseValide = false;
            while (!reponseValide)
            {
                Console.Write("Réponds par 'oui' ou 'non' : ");
                string reponse = Console.ReadLine().ToLower().Trim();
                if (reponse == "oui" || reponse == "o") { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"🎉 Super ! Bon retour parmi nous {joueurSimilaire.Nom} !"); Console.ResetColor(); return joueurSimilaire.Nom; }
                else if (reponse == "non" || reponse == "n") { Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine($"👋 Bienvenue à toi {nom} !"); Console.ResetColor(); reponseValide = true; }
                else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("❌ Réponds par 'oui' ou 'non'."); Console.ResetColor(); }
            }
        }
        return nom;
    }

    public static Difficulte SelectionnerDifficulte()
    {
        int[] maxValues = { 10, 30, 90, 100 }; string[] noms = { "Facile", "Moyen", "Difficile", "Expert" };
        Difficulte[] difficultes = new Difficulte[4];
        for (int i = 0; i < 4; i++) difficultes[i] = new Difficulte { Nom = noms[i], Max = maxValues[i], Vies = (int)Math.Ceiling(Math.Log(maxValues[i] + 1, 2)), Indices = Math.Max(1, (int)Math.Ceiling(Math.Log(maxValues[i] + 1, 2)) - 2) };

        Difficulte difficulteChoisie = new Difficulte(); bool choixValide = false;
        while (!choixValide)
        {
            Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine("\n--- Sélection de la difficulté 🎚️ ---");
            for (int i = 0; i < difficultes.Length; i++) Console.WriteLine($"{i + 1}. {difficultes[i].Nom} (0-{difficultes[i].Max}, {difficultes[i].Vies} vies)");
            Console.ResetColor(); Console.Write("Choisis un niveau (1-4) : ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= difficultes.Length) { difficulteChoisie = difficultes[index - 1]; choixValide = true; }
            else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("❌ Choix invalide."); Console.ResetColor(); }
        }
        return difficulteChoisie;
    }

    public static void JoueurDevine(string nomJoueur)
    {
        bool rejouer = true;
        while (rejouer)
        {
            Difficulte difficulte = SelectionnerDifficulte();
            Console.WriteLine($"\n*** Mode : Joueur devine le nombre 🎯 ({difficulte.Nom}) ***");
            Console.WriteLine($"📊 Plage : [0, {difficulte.Max}]. ❤️ Vies : {difficulte.Vies}");

            Stopwatch chrono = Stopwatch.StartNew(); Random random = new Random();
            int nombreSecret = random.Next(0, difficulte.Max + 1); int viesRestantes = difficulte.Vies; int indicesRestants = difficulte.Indices; bool trouve = false;

            Console.WriteLine($"\n🤖 J'ai choisi un nombre entre 0 et {difficulte.Max}. À toi de deviner !");

            while (viesRestantes > 0 && !trouve)
            {
                Console.WriteLine($"\n❤️ Vies restantes : {viesRestantes}");
                if (indicesRestants > 0 && viesRestantes <= difficulte.Vies - (difficulte.Indices - indicesRestants) * (difficulte.Vies / difficulte.Indices))
                {
                    int pas = (difficulte.Max + 1) / (difficulte.Indices + 1);
                    int minIndice = Math.Max(0, nombreSecret - pas * indicesRestants);
                    int maxIndice = Math.Min(difficulte.Max, nombreSecret + pas * indicesRestants);
                    Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine($"💡 Indice ({indicesRestants} restants) : Le nombre est entre {minIndice} et {maxIndice}"); Console.ResetColor(); indicesRestants--;
                }

                Console.Write("🎯 Entre ta proposition : "); string input = Console.ReadLine();
                if (int.TryParse(input, out int proposition))
                {
                    if (proposition == nombreSecret)
                    {
                        chrono.Stop(); Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n🎉 Félicitations {nomJoueur} ! Tu as trouvé le nombre {nombreSecret} en {chrono.Elapsed.TotalSeconds:F2}s !");
                        Console.ResetColor(); trouve = true; MettreAJourClassement(nomJoueur, chrono.Elapsed.TotalSeconds, viesRestantes, difficulte.Max);
                    }
                    else
                    {
                        viesRestantes--; Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("❌ Mauvaise réponse.");
                        Console.WriteLine(proposition < nombreSecret ? "📈 Le nombre est plus grand." : "📉 Le nombre est plus petit."); Console.ResetColor();
                    }
                }
                else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("❌ Entrée invalide."); Console.ResetColor(); }
            }

            if (!trouve)
            {
                chrono.Stop(); Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n😔 Désolé {nomJoueur}, le nombre était {nombreSecret}.");
                Console.ResetColor(); MettreAJourClassement(nomJoueur, chrono.Elapsed.TotalSeconds, 0, difficulte.Max);
                rejouer = ProposerRevanche();
            }
            else rejouer = false;
        }
    }

    public static void MachineDevine(string nomJoueur)
    {
        bool rejouer = true;
        while (rejouer)
        {
            Difficulte difficulte = SelectionnerDifficulte();
            Console.WriteLine($"\n*** Mode : Machine devine le nombre 🤖 ({difficulte.Nom}) ***");
            Console.WriteLine($"📊 Plage : [0, {difficulte.Max}]. ❤️ Vies machine : {difficulte.Vies}");

            Console.WriteLine($"\n🎯 Choisis un nombre secret entre 0 et {difficulte.Max}.");
            Console.WriteLine("💡 Réponds : '=' (correct), '+' (plus grand), '-' (plus petit)");

            int min = 0, max = difficulte.Max, viesRestantes = difficulte.Vies; bool trouve = false; Stopwatch chrono = Stopwatch.StartNew();

            while (viesRestantes > 0 && !trouve)
            {
                int proposition = min + (max - min) / 2;
                Console.WriteLine($"\n❤️ Vies machine : {viesRestantes}"); Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"🤖 Proposition : {proposition} ? (=/+/-) : "); Console.ResetColor();
                string reponse = Console.ReadLine().Trim();

                if (reponse == "=") { chrono.Stop(); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"\n🎉 Bravo {nomJoueur} ! Machine a trouvé {proposition} !"); Console.ResetColor(); trouve = true; MettreAJourClassement(nomJoueur, chrono.Elapsed.TotalSeconds, 0, difficulte.Max); }
                else if (reponse == "+") { min = proposition + 1; viesRestantes--; Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("📈 Plus grand."); Console.ResetColor(); }
                else if (reponse == "-") { max = proposition - 1; viesRestantes--; Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("📉 Plus petit."); Console.ResetColor(); }
                else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("❌ Réponse invalide."); Console.ResetColor(); }
                if (min > max) { chrono.Stop(); Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("\n⚠️ Incohérence dans tes réponses !"); Console.ResetColor(); break; }
            }

            if (!trouve) { chrono.Stop(); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"\n🎉 Félicitations {nomJoueur}, tu as gagné !"); Console.ResetColor(); MettreAJourClassement(nomJoueur, chrono.Elapsed.TotalSeconds, viesRestantes, difficulte.Max); }
            rejouer = ProposerRevanche();
        }
    }

    public static bool ProposerRevanche()
    {
        Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine("\n╔══════════════════════════════╗"); Console.WriteLine("║    VEUX-TU UNE REVANCHE ?   ║"); Console.WriteLine("║    🎯 Cette fois tu gagnes !║"); Console.WriteLine("╚══════════════════════════════╝"); Console.ResetColor();
        bool reponseValide = false; while (!reponseValide)
        {
            Console.Write("Réponds (oui/non) : "); string reponse = Console.ReadLine().ToLower().Trim();
            if (reponse == "oui" || reponse == "o") { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("\n🎮 C'est reparti !"); Console.ResetColor(); return true; }
            else if (reponse == "non" || reponse == "n") { Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine("\n👋 À la prochaine !"); Console.ResetColor(); return false; }
            else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("❌ 'oui' ou 'non'."); Console.ResetColor(); }
        }
        return false;
    }

    public static List<ScoreboardEntry> ChargerClassement()
    {
        if (!File.Exists(FICHIER_CLASSEMENT)) return new List<ScoreboardEntry>();
        try
        {
            List<ScoreboardEntry> classement = new List<ScoreboardEntry>(); string[] lignes = File.ReadAllLines(FICHIER_CLASSEMENT);
            for (int i = 1; i < lignes.Length; i++) { try { string[] colonnes = ParseCsvLine(lignes[i]); if (colonnes.Length >= 4) classement.Add(new ScoreboardEntry { Nom = colonnes[0].Trim().Trim('"'), TempsMeilleur = double.Parse(colonnes[1].Trim().Trim('"'), CultureInfo.InvariantCulture), ViesRestantes = int.Parse(colonnes[2].Trim().Trim('"')), MaxAtteint = int.Parse(colonnes[3].Trim().Trim('"')) }); } catch { } }
            return classement;
        }
        catch { return new List<ScoreboardEntry>(); }
    }

    private static string[] ParseCsvLine(string ligne)
    {
        List<string> colonnes = new List<string>(); bool dansGuillemets = false; string colonneCourante = "";
        for (int i = 0; i < ligne.Length; i++)
        {
            char c = ligne[i];
            if (c == '"') dansGuillemets = !dansGuillemets;
            else if (c == ',' && !dansGuillemets) { colonnes.Add(colonneCourante); colonneCourante = ""; }
            else colonneCourante += c;
        }
        colonnes.Add(colonneCourante); return colonnes.ToArray();
    }

    public static void SauvegarderClassement(List<ScoreboardEntry> classement)
    {
        try
        {
            var classementTrie = classement.OrderByDescending(e => e.MaxAtteint).ThenByDescending(e => e.ViesRestantes).ThenBy(e => e.TempsMeilleur).ToList();
            using (StreamWriter writer = new StreamWriter(FICHIER_CLASSEMENT))
            {
                writer.WriteLine("Nom,TempsMeilleur,ViesRestantes,MaxAtteint");
                foreach (var entry in classementTrie) writer.WriteLine($"{(entry.Nom.Contains(",") ? $"\"{entry.Nom}\"" : entry.Nom)},{entry.TempsMeilleur.ToString("F2", CultureInfo.InvariantCulture)},{entry.ViesRestantes},{entry.MaxAtteint}");
            }
        }
        catch { }
    }

    public static void MettreAJourClassement(string nomJoueur, double temps, int viesRestantes, int maxAtteint)
    {
        List<ScoreboardEntry> classement = ChargerClassement();
        string nomNormalise = new string(nomJoueur.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower();
        ScoreboardEntry joueurExistant = classement.FirstOrDefault(e => new string(e.Nom.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower() == nomNormalise);

        if (joueurExistant != null)
        {
            bool meilleurScore = maxAtteint > joueurExistant.MaxAtteint || (maxAtteint == joueurExistant.MaxAtteint && viesRestantes > joueurExistant.ViesRestantes) || (maxAtteint == joueurExistant.MaxAtteint && viesRestantes == joueurExistant.ViesRestantes && temps < joueurExistant.TempsMeilleur);
            if (meilleurScore) { joueurExistant.TempsMeilleur = temps; joueurExistant.ViesRestantes = viesRestantes; joueurExistant.MaxAtteint = maxAtteint; Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"\n⭐ Nouveau meilleur score !"); Console.ResetColor(); }
            else { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"\nℹ️ Score actuel gardé."); Console.ResetColor(); }
        }
        else { classement.Add(new ScoreboardEntry { Nom = nomJoueur, TempsMeilleur = temps, ViesRestantes = viesRestantes, MaxAtteint = maxAtteint }); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"\n👤 Nouveau joueur ajouté !"); Console.ResetColor(); }
        SauvegarderClassement(classement);
    }

    public static void AfficherClassement()
    {
        List<ScoreboardEntry> classement = ChargerClassement();
        var classementTrie = classement.OrderByDescending(e => e.MaxAtteint).ThenByDescending(e => e.ViesRestantes).ThenBy(e => e.TempsMeilleur).ToList();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n" + new string('═', 80));
        Console.WriteLine("                         🏆 CLASSEMENT COMPLET 🏆");
        Console.WriteLine(new string('═', 80));
        Console.WriteLine("┌──────┬────────────────────┬────────────┬───────────────┬────────────┐");
        Console.WriteLine("│ Rang │        Nom         │ Niveau Max │ Vies Restantes │ Temps (s)  │");
        Console.WriteLine("├──────┼────────────────────┼────────────┼───────────────┼────────────┤");
        Console.ResetColor();

        if (classementTrie.Count == 0)
        {
            Console.WriteLine("│                     Aucun joueur encore inscrit                     │");
        }
        else
        {
            for (int i = 0; i < classementTrie.Count; i++)
            {
                ScoreboardEntry entry = classementTrie[i];
                ConsoleColor couleur = i == 0 ? ConsoleColor.Green : i == 1 ? ConsoleColor.Yellow : i == 2 ? COULEUR_BRONZE : ConsoleColor.White;

                Console.ForegroundColor = couleur;
                string ligne = $"│ {i + 1,-4} │ {entry.Nom,-18} │ {entry.MaxAtteint,-10} │ {entry.ViesRestantes,-13} │ {entry.TempsMeilleur,-10:F2} │";
                Console.WriteLine(ligne);
                Console.ResetColor();
            }
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("└──────┴────────────────────┴────────────┴───────────────┴────────────┘");
        Console.WriteLine(new string('═', 80));
        Console.ResetColor();

        Console.WriteLine("\n📈 Statistiques du classement :");
        Console.WriteLine($"   • Total de joueurs : {classementTrie.Count}");
        if (classementTrie.Count > 0)
        {
            Console.WriteLine($"   • Meilleur niveau : {classementTrie[0].MaxAtteint}");
            Console.WriteLine($"   • Meilleur temps : {classementTrie[0].TempsMeilleur:F2}s");
        }
        Console.WriteLine("\n↵ Appuyez sur Entrée pour continuer...");
        Console.ReadLine();
    }
}