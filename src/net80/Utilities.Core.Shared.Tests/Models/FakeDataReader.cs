using System.Data;

namespace Utilities.Core.Shared.Tests;

internal class FakeDataReader : IDataReader
{
    private readonly List<Dictionary<string, object>> _rows;
    private int _index = -1;

    public FakeDataReader(List<Dictionary<string, object>> rows)
    {
        _rows = rows;
    }

    public bool Read()
    {
        _index++;
        return _index < _rows.Count;
    }

    public object this[string name] => _rows[_index][name];

    public object this[int i] => throw new NotImplementedException();

    public bool IsDBNull(int i)
    {
        var key = _rows[_index].Keys.ElementAt(i);
        return _rows[_index][key] == DBNull.Value;
    }

    public int GetOrdinal(string name)
    {
        return _rows[_index].Keys.ToList().IndexOf(name);
    }

    public void Dispose() { }
    public void Close() { }
    public bool NextResult() => false;
    public int Depth => 0;
    public bool IsClosed => false;
    public int RecordsAffected => -1;
    public DataTable GetSchemaTable() => null;

    #region Unused IDataReader methods
    public int FieldCount => _rows[_index].Count;
    public bool GetBoolean(int i) => (bool)this[i];
    public byte GetByte(int i) => (byte)this[i];
    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => throw new NotImplementedException();
    public char GetChar(int i) => (char)this[i];
    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) => throw new NotImplementedException();
    public IDataReader GetData(int i) => throw new NotImplementedException();
    public string GetDataTypeName(int i) => throw new NotImplementedException();
    public DateTime GetDateTime(int i) => (DateTime)this[i];
    public decimal GetDecimal(int i) => (decimal)this[i];
    public double GetDouble(int i) => (double)this[i];
    public Type GetFieldType(int i) => this[i].GetType();
    public float GetFloat(int i) => (float)this[i];
    public Guid GetGuid(int i) => (Guid)this[i];
    public short GetInt16(int i) => (short)this[i];
    public int GetInt32(int i) => (int)this[i];
    public long GetInt64(int i) => (long)this[i];
    public string GetName(int i) => _rows[_index].Keys.ElementAt(i);
    public string GetString(int i) => (string)this[i];
    public object GetValue(int i) => this[i];
    public int GetValues(object[] values) => throw new NotImplementedException();
    public bool IsDBNull(string name) => this[name] == DBNull.Value;
    #endregion
}
