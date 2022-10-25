# notes

## Performances

Après une étude *presque* approfondie, on conclu les faits suivants:

- sur le bitmap en direct, les boucles `for` sont plus lentes que les fonctions récursives équivalentes (*cf* exemple [for loops vs recursion](#for-loops-vs-recursion)),
- les *pipes* sont plus lents que les appels directs des fonctions (*cf* exemple [Pipes vs direct calls](#Pipes-vs-direct-calls))

## Traductions

L'idée:

- partir d'un fichier CSV par langue contenant une liste de données au format:
  - ID unique,
  - le contenu de la chaîne
- ce fichier est traduit pour obtenir un code source F# avec un dictionnare contenant le couple ID, chaîne.

## BitmapTools.LockContext, refactoring
Le but ultime: faire un *double-buffer* indépendant du bitmap sur lequel s'opère les transformations de l'image sauf s'il est plus simple d'agir directement sur le bitmap (comme, par exemple, la rotation de l'image). Cela pourra peut-être permettre le recadrage et le zoom.

Pour en arriver là, il faut un certain nombre de conditions. Tout d'abord, il faut arrêter de créer des objets `LockContext`comme cela est fait actuellement. L'utilisation  de l'`Option` est impérative. A noter que cela peut être intéressant de faire un module spécifique à la manipulation des types et des `Option`, mais il faut que je *révise* le fonctionnement des *types* en *F#*.


## exemples de code

### for loops vs recursion

ce code est lent, pas beau et nécessite des variables *mutables*:

```f#
for k = 0 to bitmapLenInBytes - 1 do
  let address = NativePtr.add<byte> (NativePtr.ofNativeInt data.Value.Scan0) idx
  setRGB address (f (getRGB address))
  idx <- idx + sizeofColor
```

ce code est plus rapide:

```f#
let rec loop idx k =
  if k < bitmapLenInBytes then
     let address = NativePtr.add<byte> (NativePtr.ofNativeInt data.Value.Scan0) idx
     setRGB address (f (getRGB address))
     loop (idx + sizeofColor) (k + 1)
  else
     ()
loop 0 0
```

### Pipes vs direct calls

ce code est beau mais lent:

```f#
     getRGB address |> f |> setRGB address
```

ce code est moins beau mais plus rapide:

```f#
     setRGB address (f (getRGB address))
```
