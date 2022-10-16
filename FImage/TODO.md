# TODO

(Sorry, no english version)

## fonctionalités
- ***DONE*** Sauvegarde de l'image
- ***TODO*** Afficher les métadonnées de l'image
- ***DONE*** Afficher une page d'aide, F1

## distribution
- ***DONE*** faire un `.zip`pour *Windows*
- ***DONE*** numéro de version
- ***DONE*** s'assurer de produire du code 64 bits

## code

### `BitmapTools`
- ***DONE*** Supprimer l'inutile
- ***DONE*** revoir `LockContext`, proposer quelque chose comme `withLockContext`

### `ThePicture`
- ***PARTIAL*** Modifier, voire supprimer la machine d'état
  - la machine d'état est plus adaptée mais pas parfaite
- ***DONE*** Supprimer l'inutile

### en général
- ***PARTIAL*** Logging, timing uniquement si l'option de ligne de commande le permet ou si debug il y a
  - ***DONE*** avec option de compilation: `-p:DefineConstants=LOGGER`
    - il suffit d'ajouter des `#if LOGGER`, `#else` et `#endif`, ça marche pas mal

## performances
- ***TODO*** S'assurer de la consommation de mémoire

## documentation
- ***TODO*** Un *vrai* README.md
- ***TODO*** Créer des *Github pages*
- ***TODO*** Documentation utilisateur
- ***TODO*** Documentation code
