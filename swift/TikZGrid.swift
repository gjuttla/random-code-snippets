let gridDim = (4, 4)
let cellWidth = 0.5
let cellCenter = cellWidth / 2.0
var val = 0

print("\\begin{tikzpicture}[font=\\scriptsize, baseline={([yshift=-.5ex]current bounding box.center)}]")
print("\\draw[step=\(cellWidth)cm,color=gray] (0,0) grid (\(cellWidth * Double(gridDim.0)), \(cellWidth * Double(gridDim.1)));")

for i in stride(from: gridDim.0 - 1, through: 0, by: -1) {
    for j in stride(from: 0, to: gridDim.0, by: 1) {
        let x = Double(j + 1) * cellWidth - cellCenter
        let y = Double(i + 1) * cellWidth - cellCenter
        val += 1
        print("\\node at (\(x),\(y)) {\(val)};")
    }
}

print("\\end{tikzpicture}")