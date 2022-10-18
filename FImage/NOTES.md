# notes

## Performances

Après une étude *presque* approfondie, on conclu les faits suivants:
- sur le bitmap en direct, les boucles `for` sont plus lentes que les fonctions récursives équivalentes (*cf* exemple [for loops vs recursion](#for-loops-vs-recursion)),
- les *pipes* sont plus lents que les appels directs des fonctions (*cf* exemple [Pipes vs direct calls](#Pipes-vs-direct-calls))

## exemples de code

### for loops vs recursion

ce code est lent:
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

ce code est lent:
```f#
             getRGB address |> f |> setRGB address
```
ce code est plus rapide:
```f#
             setRGB address (f (getRGB address))
```
