//using System.Web.Script.Serialization;


namespace LitePlacer {
    public class LitePlacerSettings {

   



        // =================================================================================
        // Saving and restoring data tables (Note: Not job files)
        // =================================================================================
        /*     private void SaveDataGrid(string FileName, DataGridView dgv) {
                 try {
                     using (BinaryWriter bw = new BinaryWriter(File.Open(FileName, FileMode.Create))) {
                         bw.Write(dgv.Columns.Count);
                         bw.Write(dgv.Rows.Count);
                         foreach (DataGridViewRow dgvR in dgv.Rows) {
                             for (int j = 0; j < dgv.Columns.Count; ++j) {
                                 object val = dgvR.Cells[j].Value;
                                 if (val == null) {
                                     bw.Write(false);
                                     bw.Write(false);
                                 } else {
                                     bw.Write(true);
                                     bw.Write(val.ToString());
                                 }
                             }
                         }
                     }
                 } catch (System.Exception excep) {
                     MessageBox.Show(excep.Message);
                 }
             }

             private void LoadDataGrid(string FileName, DataGridView dgv) {
                 try {
                     if (!File.Exists(FileName)) {
                         return;
                     }
                     dgv.Rows.Clear();
                     using (BinaryReader bw = new BinaryReader(File.Open(FileName, FileMode.Open))) {
                         int n = bw.ReadInt32();
                         int m = bw.ReadInt32();
                         if (dgv.AllowUserToAddRows) {
                             // There is an empty row in the bottom that is visible for manual add.
                             // It is saved in the file. It is automatically added, so we don't want to add it also.
                             // It is not there when rows are added only programmatically, so we need to do it here.
                             m = m - 1;
                         }
                         for (int i = 0; i < m; ++i) {
                             dgv.Rows.Add();
                             for (int j = 0; j < n; ++j) {
                                 if (bw.ReadBoolean()) {
                                     dgv.Rows[i].Cells[j].Value = bw.ReadString();
                                 } else bw.ReadBoolean();
                             }
                         }
                     }
                 } catch (System.Exception excep) {
                     MessageBox.Show(excep.Message);
                 }
             }*/


    }
}

