# **Devine le nombre**

En quoi consiste le projet ?

Jeu console en C# dans lequel :



•	le joueur doit deviner un nombre choisi par la machine ;

•	la machine doit deviner le nombre choisi par le joueur ;

•	plusieurs difficultés sont disponibles ;

•	le joueur dispose de vies et d’indices ;

•	un classement est sauvegardé dans score.csv.



Ce qui semble déjà réalisé

•	Menu principal.

•	Demande et validation du nom du joueur.

•	Niveaux Facile, Moyen, Difficile et Expert.

•	Mode joueur contre machine.

•	Mode machine contre joueur avec recherche par intervalle.

•	Chronomètre.

•	Classement persistant en CSV.

•	Gestion de revanche et affichage du classement.

•	Interface console travaillée avec couleurs, caractères graphiques et messages.



Ce qui manque ou doit être corrigé

•	Le mode « machine devine » affiche un message de félicitations et met à jour le classement même lorsque la machine échoue ou lorsque les réponses du joueur rendent l’intervalle incohérent. Il faut distinguer victoire, défaite et triche/incohérence.

•	Les réponses + et - devraient être contrôlées pour empêcher une contradiction avec les propositions précédentes.

•	Le classement ne différencie pas toujours clairement le mode de jeu et le niveau ; il faut définir une règle de score documentée.

•	Les exceptions de lecture/écriture du fichier CSV sont silencieusement ignorées. L’utilisateur doit être informé si le classement ne peut pas être sauvegardé.

•	Les entrées sont principalement prévues en anglais informatique (oui/non, +/-) ou en français selon les écrans : uniformiser l’interface.

•	Ajouter des tests sur les bornes 0, les vies épuisées, les noms avec virgule et les fichiers CSV corrompus.

•	Ajouter une documentation de lancement et éventuellement un fichier de score d’exemple séparé des sources.



Niveau estimé

Projet fonctionnel et jouable, avec quelques incohérences de logique dans le mode où la machine devine.

