using System;
using System.Collections.Generic;
using System.Text;


class CodeGenerator
{
    StringBuilder sb = new StringBuilder();

    const int ID_START = 0;
    const int CONST_START = 0;
    const int CODE_START = 0;

    string currentAccumulator;

    int id_loc;
    int const_loc;
    int code_loc;
    int label_placeholder;
    int current_accu;

    Dictionary<string, int> idTable;
    Dictionary<string, int> labelTable;
    Dictionary<int, int> constTable;
    Dictionary<int, int> physicalLabelLocation;

    public CodeGenerator()
    {
        id_loc = ID_START;
        const_loc = CONST_START;
        code_loc = CODE_START;
        current_accu = 0;
    }

    public void WriteLine(string line)
    {
        sb.AppendFormat("{0}: ", code_loc++);
        sb.AppendLine(line);
    }

    public void OutputTest()
    {

        Console.WriteLine("Assembly Output");
        Console.WriteLine(sb.ToString());
    }

    public string GetIdAddrStr(string id)
    {
        if (labelTable.ContainsKey(id))
            return String.Empty;

        if (!idTable.ContainsKey(id))
        {
            id_loc++;
            idTable[id] = id_loc;
        }

        return idTable[id].ToString();
    }

    public string GetLabelAddrStr(string label)
    {
        if (idTable.ContainsKey(label))
            return String.Empty;

        if (!labelTable.ContainsKey(label))
        {
            labelTable[label] = 0;
        }

        return labelTable[label].ToString();
    }

    public string GetConstAddrStr(string intValue)
    {
        int value = Int32.Parse(intValue);

        if (!constTable.ContainsKey(value))
        {
            const_loc++;
            constTable[value] = const_loc;
        }

        return constTable[value].ToString();
    }

    public void SetPhysicalLabelLocation(int label)
    {
        physicalLabelLocation[label] = code_loc;
    }

    public string NewAccu()
    {
        ++current_accu;
        return GetIdAddrStr(current_accu.ToString());
    }

    public void FreeAccu()
    {
        --current_accu;
    }


}

