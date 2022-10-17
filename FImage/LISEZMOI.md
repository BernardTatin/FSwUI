# FImage, pour jouer avec les couleurs

C'est un logiciel en cours de développement qui a des défauts et, très certainement, des bugs. Il est même incomplet sur certains points. Il est donc ***fortement*** conseillé de sauvegarder les images que vous voulez traiter avec cette application.

Le but de cette application est de pouvoir modifier des images au niveau des couleurs, teintes, saturations et quelques autres paramètres du même type. Les filtres proposés actuellement sont bruts de décoffrage et ne sont pas paramétrables. Mais ça va changer.

## démarrage rapide

- Assurez-vous d'avoir un ***Windows 10*** bien à jour,
- Téléchargez le [fichier ZIP](https://bernardtatin.github.io/somefiles/FImage%20for%20net48.zip),
- Décompressez le fichier Zip où vous voulez,
- Double cliquez sur le fichier `FImage.exe`,
- C'est parti.

## configuration requise

Pour le moment, il fonctionne uniquement sur un ***Windows 10*** *64 bits*. Dans un avenir proche, une version **Linux**__ verra le jour. Et si tout va bien, il pourrait y avoir une version pour **Mac**__.

Ce logiciel consomme relativement peu de mémoire. Cependant, je pense qu'il est nécessaire d'avoir au moins 8 Go de RAM.

## installation

Téléchargez le [fichier ZIP](https://bernardtatin.github.io/somefiles/FImage%20for%20net48.zip), décompressez le dans le répertoire de votre choix.

L'installation est finie.

## lancer l'application

Une fois installé, sélectionnez le fichier `FImage.exe` puis double cliquez le pour le lancer.

## utilisation

Tout est dans les menus. A terme, il y aura une description détaillée de la chose avec de belles images.

### opérations sue les fichiers

- <Ctrl+O>: ouvre un fichier,
- <Ctrl+S>: sauve un fichier,
- <Ctrl+Q>: ferme l'application
    - ***ATTENTION***: si un fichier est modifié, l'application se ferme sans prévenir, toutes les modifications seront perdues! Une prochaine version permettra de modifier se comportement.
- <Ctrl+D>: permet de recharger le fichier en cours, toutes les modifications sont perdues.

### les filtres

Les filtres actuels sont assez primitifs et ne travaillent que sur les canaux RVB de l'image.

- <Ctrl+R>: effectue une rotation de l'image de 90° dans le sens des aiguilles d'une montre (ok, c'est pas un filtre, mais ça sert),
- <Ctrl+T>: effectue une rotation des canaux RVB vers la droite,    c'est à dire que RVB devient BRV,
- <Ctrl+L>: effectue une rotation des canaux RVB vers la gauche,    c'est à dire que RVB devient VBR,
- <Ctrl+G>, <Ctrl+K> et <Ctrl+H>: produit une image à deux *couleurs* uniquement, noir pur et blanc pur, avec des seuils différents pour chaque racourcis; effet lithographie, tract 1968, ... et j'aime beaucoup (explications détaillées plus tard, mais merci de m'avoir posé la question)
- <Ctrl+X>, <Ctrl+C>, <Ctrl+V>, <Ctrl+M>, <Ctrl+N>: met en valeur la couleur la plus *lumineuse* avec, pour chaque raccourci, un seuil différent. Amusant, très amusant et oui, il y aura des explications plus tard.
