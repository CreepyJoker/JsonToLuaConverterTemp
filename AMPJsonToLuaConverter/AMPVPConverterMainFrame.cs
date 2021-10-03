using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows.Forms;

namespace ArenaMasterIO_Json_Converter
{
    public partial class MainFrame : Form
    {
        public MainFrame()
        {
            InitializeComponent();
        }
        private string jsonString = "";
        private string outputLUA = "";
        private string regionFromFile = "";
        private string origFileName = "";
        private void MainFrame_Load(object sender, EventArgs e)
        {
            label1.Text = "To start, load a JSON file first.";
            convertBtn.Enabled = false;
        }

        private void loadJsonBtn_Click(object sender, EventArgs e)
        {
            AMPVP_ResetUI();

            var dlg = new OpenFileDialog();
            
            dlg.DefaultExt = ".json";
            dlg.Filter = "Json Files (.json)|*.json";
            var result = dlg.ShowDialog();
            if (result != DialogResult.OK) return;


            using (StreamReader sr = new StreamReader(dlg.FileName))
            {
                label1.Text = "Loading file, please wait...";

                regionFromFile = dlg.SafeFileName.Replace(".json", "");
                origFileName = dlg.FileName.Replace(".json", "");
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    jsonString = line;
                    //Console.WriteLine(regionFromFile + ": " + jsonString);
                }
                convertBtn.Enabled = true;
                label1.Text = "File loaded succesfully. You can now Convert it.";
                sr.Close();
            }
            
        }

        private void convertBtn_Click(object sender, EventArgs e)
        {
            AMPVP_ConvertJSONTableFunc(jsonString);
        }

        private void AMPVP_ResetUI()
        {
            convertBtn.Enabled = false;
            label1.Text = "To start, load a JSON file first.";
            outputLUA = "";
            jsonString = "";
            origFileName = "";
            regionFromFile = "";
        }

        private void AMPVP_ConvertJSONTableFunc(string json)
        {
            
            try
            {
                
                var objects = JsonConvert.DeserializeObject<dynamic>(json);

                using (StreamWriter sw = new StreamWriter(origFileName + ".lua"))
                {
                    
                    string startLua = "AMPVP_REGIONDATA_" + regionFromFile + " = { ";

                    sw.WriteLine(startLua);

                    foreach (var obj in objects)
                    {
                        var userName = obj.name;
                        var realm = obj.realm;
                        var twosExp = obj.exp_2v2;
                        var tresExp = obj.exp_3v3;
                        var RBGExp = obj.exp_rbg;
                        var latest_update = obj.updated_at;

                        var acc_2v2exp = obj.acc_exp_2v2;
                        var acc_3v3exp = obj.acc_exp_3v3;
                        var acc_rbgexp = obj.acc_exp_rbg;

                        if (twosExp == null)
                        {
                            twosExp = 0;
                        }

                        if (tresExp == null)
                        {
                            tresExp = 0;
                        }

                        if (RBGExp == null)
                        {
                            RBGExp = 0;
                        }

                        if (acc_2v2exp == null && acc_3v3exp == null && acc_rbgexp == null)
                        {
                            outputLUA = "[\"" + userName + "-" + realm + "\"]={"
                            + "[\"e2s\"]=" + twosExp + ","
                            + "[\"e3s\"]=" + tresExp + ","
                            + "[\"eRBG\"]=" + RBGExp + ","
                            + "[\"lu\"]=" + "\"" + latest_update + "\""
                            + "},"
                            ;
                        }

                        if(acc_2v2exp != null && acc_3v3exp == null && acc_rbgexp == null)
                        {
                            outputLUA = "[\"" + userName + "-" + realm + "\"]={"
                            + "[\"e2s\"]=" + twosExp + ","
                            + "[\"a2s\"]=" + acc_2v2exp + ","
                            + "[\"e3s\"]=" + tresExp + ","
                            + "[\"eRBG\"]=" + RBGExp + ","
                            + "[\"lu\"]=" + "\"" + latest_update + "\""
                            + "},"
                            ;
                        }

                        if (acc_2v2exp == null && acc_3v3exp != null && acc_rbgexp == null)
                        {
                            outputLUA = "[\"" + userName + "-" + realm + "\"]={"
                            + "[\"e2s\"]=" + twosExp + ","
                            + "[\"a3s\"]=" + acc_3v3exp + ","
                            + "[\"e3s\"]=" + tresExp + ","
                            + "[\"eRBG\"]=" + RBGExp + ","
                            + "[\"lu\"]=" + "\"" + latest_update + "\""
                            + "},"
                            ;
                        }

                        if (acc_2v2exp == null && acc_3v3exp == null && acc_rbgexp != null)
                        {
                            outputLUA = "[\"" + userName + "-" + realm + "\"]={"
                            + "[\"e2s\"]=" + twosExp + ","
                            + "[\"e3s\"]=" + tresExp + ","
                            + "[\"eRBG\"]=" + RBGExp + ","
                            + "[\"aRBG\"]=" + acc_rbgexp + ","
                            + "[\"lu\"]=" + "\"" + latest_update + "\""
                            + "},"
                            ;
                        }


                        if (acc_2v2exp != null && acc_3v3exp != null && acc_rbgexp != null)
                        {
                            outputLUA = "[\"" + userName + "-" + realm + "\"]={"
                            + "[\"e2s\"]=" + twosExp + ","
                            + "[\"a2s\"]=" + acc_2v2exp + ","
                            + "[\"e3s\"]=" + tresExp + ","
                            + "[\"a3s\"]=" + acc_3v3exp + ","
                            + "[\"eRBG\"]=" + RBGExp + ","
                            + "[\"aRBG\"]=" + acc_rbgexp + ","
                            + "[\"lu\"]=" + "\"" + latest_update + "\""
                            + "},"
                            ;
                        }


                        string line = outputLUA;

                        if (outputLUA != null && outputLUA != "")
                        {
                            sw.WriteLine(line);
                        }
                    }

                    string endLua = "}";
                    sw.WriteLine(endLua);
                    sw.Close();
                    AMPVP_ResetUI();
                    MessageBox.Show("File converted succesfully. It can be found in the same folder as the JSON File.", "AMPVP Data Converter", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch(Exception ex)
            {
                label1.Text = "An error occured, aborting...";

                Console.WriteLine(ex.Message);
            }
        }


        private void AMPVP_ConvertJSONTableFunc2(string json)
        {

            try
            {

                var jsonLinq = JObject.Parse(json);

                var tempOutput = "";

                var srcArray = jsonLinq.SelectToken("realms");

                using (StreamWriter sw = new StreamWriter(origFileName + ".lua"))
                {

                    string startLua = "AMPVP_REALMLIST = { ";

                    sw.WriteLine(startLua);

                    foreach (var idk in srcArray)
                    {
                        //Console.WriteLine(idk);
                        var realmName = idk.SelectToken("name");
                        var realmSlug = idk.SelectToken("slug");
                        Console.WriteLine(realmName + " - " + realmSlug);
                        tempOutput = "[\"" + realmName + "\"]" + " = \"" + realmSlug + "\",";

                        if (tempOutput != null && tempOutput != "")
                        {
                            sw.WriteLine(tempOutput);
                        }

                    }

                   // Console.WriteLine(srcArray);
                    

                    string endLua = "}";
                    sw.WriteLine(endLua);
                    sw.Close();
                    AMPVP_ResetUI();
                    MessageBox.Show("File converted succesfully. It can be found in the same folder as the JSON File.", "AMPVP Data Converter", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

            }
            catch (Exception ex)
            {
                label1.Text = "An error occured, aborting...";

                Console.WriteLine(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AMPVP_ConvertJSONTableFunc2(jsonString);
        }
    }
}