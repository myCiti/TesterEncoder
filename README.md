# TesterEncoder

### Ce program est développé lors du stage de l'hiver 2021 par Thomas M.

Ce projet correspond au test de la logique du programme qui sert à tester les encodeurs numériques relatifs face au traditionnel vis sans fin et limite.

La logique du projet est sur un seul fichier, soit program.cs

La logique est une fonction qui s'exécute en permanence. 
Dans cette fonction, il y a un switch case qui est concu selon une machine à état qui est représenté graphiquement dans le fichier TesterEncoderV6StateMachine.png .
Elle intègre la création de fichiers .csv qui permet de garder en mémoire le nombre de pulse et d'index lors d'un demi-cycle.
