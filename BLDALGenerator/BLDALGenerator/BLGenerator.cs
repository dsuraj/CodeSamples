using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLDALGenerator
{
    public partial class BLGenerator : Form
    {
        #region "Properties"

        private string ProperCaseFileName
        {
            get
            {
                if (ddlTable.SelectedItem != null)
                {
                    return (char.ToUpper(Convert.ToString(ddlTable.SelectedItem)[0]) + Convert.ToString(ddlTable.SelectedItem).Substring(1)).Replace(" ", string.Empty);
                }
                return string.Empty;
            }
        }

        #endregion "Properties"

        public BLGenerator()
        {
            InitializeComponent();

            ddlDatabase.Enabled = false;
            ddlTable.Enabled = false;
            txtID.Focus();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtID.Text.Trim()) && !string.IsNullOrEmpty(txtPassword.Text.Trim()) && !string.IsNullOrEmpty(txtServer.Text.Trim()))
            {
                FillDatabase();
            }
            else
            {
                MessageBox.Show("ID/Password/Server Name can't be blank.");
            }
        }

        private void FillDatabase()
        {
            //Get DB List

            var connectionString = string.Format("Data Source={0};User ID={1};Password={2};", txtServer.Text.Trim(), txtID.Text.Trim(), txtPassword.Text.Trim());

            DataTable databases = null;
            ddlDatabase.Items.Clear();
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                try
                {
                    databases = sqlConnection.GetSchema("Databases");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                sqlConnection.Close();
            }

            if (databases != null && databases.Rows.Count > 0)
            {
                foreach (DataRow database in databases.Rows)
                {
                    String databaseName = database.Field<String>("database_name");
                    ddlDatabase.Items.Add(databaseName);
                }

                ddlDatabase.Enabled = true;
                ddlDatabase.SelectedIndex = 0;
                FillTables();
            }
        }

        private void FillTables()
        {
            //Get Table list

            var connectionString = string.Format("Data Source={0};User ID={1};Password={2};Initial Catalog={3};", txtServer.Text.Trim(), txtID.Text.Trim(), txtPassword.Text.Trim(), ddlDatabase.SelectedItem);
            ddlTable.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    DataTable table = connection.GetSchema("Tables");
                    if (table != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            if (row[2] != null && Convert.ToString(row["Table_Type"]).ToLower() == "base table")
                            {
                                string tableName = row[2].ToString();
                                ddlTable.Items.Add(tableName);
                                ddlTable.Enabled = true;
                                ddlTable.SelectedIndex = 0;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private string GetColumns()
        {
            //Get column list

            var connectionString = string.Format("Data Source={0};User ID={1};Password={2};Initial Catalog={3};", txtServer.Text.Trim(), txtID.Text.Trim(), txtPassword.Text.Trim(), ddlDatabase.SelectedItem);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    DataTable table = connection.GetSchema("Columns", new[] { Convert.ToString(ddlDatabase.SelectedItem), null, Convert.ToString(ddlTable.SelectedItem) });
                    if (table != null && table.Rows.Count > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendFormat("public class {0}BL{1}{2}{3}", ProperCaseFileName, Environment.NewLine, "{", Environment.NewLine);

                        stringBuilder.AppendFormat("{0}#region \"Properties\"{1}{2}", Environment.NewLine, Environment.NewLine, Environment.NewLine);
                        foreach (DataRow row in table.Rows)
                        {
                            if (row["Column_Name"] != null && !string.IsNullOrEmpty(Convert.ToString(row["Column_Name"])) && row["Is_Nullable"] != null && !string.IsNullOrEmpty(Convert.ToString(row["Is_Nullable"])) && row["Data_Type"] != null && !string.IsNullOrEmpty(Convert.ToString(row["Data_Type"])))
                            {
                                string columnName = Convert.ToString(row["Column_Name"]);
                                string nullableDataType = MapNullableDataType(Convert.ToString(row["Data_Type"]), Convert.ToString(row["Is_Nullable"]));

                                stringBuilder.AppendFormat("public {0} {1}{2}{3}{4}get;{5}set;{6}", nullableDataType, columnName, Environment.NewLine, "{", Environment.NewLine, Environment.NewLine, Environment.NewLine);
                                stringBuilder.Append("}");
                                stringBuilder.Append(Environment.NewLine);
                            }
                        }
                        stringBuilder.AppendFormat("{0}#endregion \"Properties\"{1}", Environment.NewLine, Environment.NewLine);
                        stringBuilder.AppendFormat("{0}#region \"Methods\"{1}{2}#endregion \"Methods\"{3}", Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine);
                        stringBuilder.Append("}");
                        return stringBuilder.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return string.Empty;
        }

        private string MapNullableDataType(string dataType, string isNullable)
        {
            isNullable = string.Compare(isNullable, "yes", true) == 0 ? "?" : string.Empty;

            if (string.Compare(dataType, "varchar", true) == 0 || string.Compare(dataType, "nvarchar", true) == 0 || string.Compare(dataType, "char", true) == 0 || string.Compare(dataType, "text", true) == 0 || string.Compare(dataType, "nchar", true) == 0 || string.Compare(dataType, "ntext", true) == 0)
            {
                return "string";
            }
            else if (string.Compare(dataType, "bit", true) == 0)
            {
                return string.Format("bool{0}", isNullable);
            }
            else if (string.Compare(dataType, "bigint", true) == 0 || string.Compare(dataType, "tinyint", true) == 0 || string.Compare(dataType, "int", true) == 0 || string.Compare(dataType, "smallint", true) == 0)
            {
                return string.Format("int{0}", isNullable);
            }
            else if (string.Compare(dataType, "decimal", true) == 0 || string.Compare(dataType, "money", true) == 0 || string.Compare(dataType, "numeric", true) == 0 || string.Compare(dataType, "float", true) == 0)
            {
                return string.Format("decimal{0}", isNullable);
            }
            else if (string.Compare(dataType, "date", true) == 0 || string.Compare(dataType, "datetime", true) == 0 || string.Compare(dataType, "datetime2", true) == 0 || string.Compare(dataType, "smalldatetime", true) == 0 || string.Compare(dataType, "time", true) == 0)
            {
                return string.Format("DateTime{0}", isNullable);
            }
            else if (string.Compare(dataType, "uniqueidentifier", true) == 0)
            {
                return string.Format("Guid{0}", isNullable);
            }

            return dataType;
        }

        private void ddlDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTables();
        }

        private void btnGenerateBL_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFolderPath.Text.Trim()))
            {
                using (StreamWriter writer = new StreamWriter(string.Format("{1}\\{0}BL.cs", ProperCaseFileName, txtFolderPath.Text), false))
                {
                    writer.WriteLine(GetColumns());
                }

                if (MessageBox.Show(string.Format("{0}BL class generated successfully. Do you want to open the file?", ProperCaseFileName), "Success", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    Process.Start(string.Format("{1}\\{0}BL.cs", ProperCaseFileName, txtFolderPath.Text));
                }
            }
            else
            {
                MessageBox.Show("Please select a folder to save the file.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK || folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                txtFolderPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void ddlTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnGenerateBL.Text = "Generate BL Class";
            if (ddlTable.SelectedItem != null)
            {
                btnGenerateBL.Text = string.Format("Generate {0}BL Class", ProperCaseFileName);
            }
        }

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {
            Process.Start("http://surajdeshpande.wordpress.com/2013/08/06/business-layer-class-generator/");
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            Process.Start("https://twitter.com/deshpandesuraj");
        }
    }
}
