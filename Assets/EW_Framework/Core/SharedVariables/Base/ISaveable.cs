namespace EW_Framework.Core.SharedVariables.Base {
    public interface ISaveable {
        // The key of the save data
        string SaveKey { get; }
        // Get the save data
        string GetSaveData();
        // Load the save data
        void LoadData(string jsonData);
    }
}