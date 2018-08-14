let gridDim = (6, 6)
let cellWidth = 0.5
let cellCenter = cellWidth / 2.0
var val = 0

for i in stride(from: gridDim.0 - 1, through: 0, by: -1) {
    for j in stride(from: 0, to: gridDim.0, by: 1) {
        let x = Double(j + 1) * cellWidth - cellCenter
        let y = Double(i + 1) * cellWidth - cellCenter
        val += 1
        print("\\node at (\(x),\(y)) {\(val)};")
    }
    print("")
}