using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using youtube_bot_gui.util;
using youtube_bot_lib.api;
using youtube_bot_lib.model;

namespace youtube_bot_gui
{

    public partial class frmMain : Form
    {


        public frmMain()
        {                  
            InitializeComponent();
        }
        public int UnFollowCount { get; set; }
        public int FollowCount { get; set; }
        private IList<User> users = new List<User>();
        IList<string>  searchStrings = new List<string>();
        IList<System.Threading.Thread>  threads = new List<Thread>();

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure About closing this program", "Closing", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                
                saveSearchbutton_Click(sender, e);
                this.Close();
            }
        }

        private void txtProxy_TextChanged(object sender, EventArgs e)
        {
            btnAddUser.
                Enabled =

                !string.IsNullOrWhiteSpace(txtpassword.Text) &&
                !string.IsNullOrWhiteSpace(txtUsername.Text);
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (!txtUsername.Text.Equals("") && !txtpassword.Text.Equals("") &&
                !appNametextBox.Text.Equals("") && !apiKeytextBox.Text.Equals(""))
            {


                User U = new User()
                             {
                                 Password = txtpassword.Text,
                                 UserName = txtUsername.Text,
                                 Proxy = txtProxy.Text,
                                 appName = appNametextBox.Text,
                                 apiKey = apiKeytextBox.Text,
                                 Enabled = true
                             };
                UserManager.saveUser(U);
                users.Add(U);
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = U.UserName;
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = U.Proxy;
                row.Cells.Add(cell);
                usersdataGridView.Rows.Add(row);
            }
            txtProxy.Text = apiKeytextBox.Text = appNametextBox.Text =txtpassword.Text = txtUsername.Text = "";
        }


        int xRoundCoutn = 0;

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Instantiate the writer
            TextWriter _writer = new TextBoxStreamWriter(this.outputtextBox);
            // Redirect the out Console stream
            Console.SetOut(_writer);
            users = YouTubeAccountPool.getUsersFromXml();            
            foreach (User user in users)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = user.UserName;
                row.Cells.Add(cell);
                cell = new DataGridViewTextBoxCell();
                cell.Value = user.Proxy;
                row.Cells.Add(cell);
                usersdataGridView.Rows.Add(row);

            }
            cmbThreadCount.SelectedIndex = 0;
            LoadSetting();
            loadSearchCriteria();
            
        }

        private void loadSearchCriteria()
        {
            try
            {
                IList<string> list = SearchCriteriaStorageManager.getSearchCriterias();
                foreach (string s in list)
                {
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Value = s;
                    searchStrings.Add(s);
                    DataGridViewRow row = new DataGridViewRow();
                    row.Cells.Add(cell);
                    cell = new DataGridViewButtonCell();
                    cell.Value = "X";
                    row.Cells.Add(cell);
                    searchGridView.Rows.Add(row);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                
            }
        }

        private void LoadSetting()
        {
            cmbThreadCount.Text = AppSetting.ThreadCount.ToString();
            minViewCount.Value = AppSetting.MinViewCount;

            delayMinute.Value = AppSetting.DelayMinute;
        }

        private void dgUserEdit_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (e.ColumnIndex == 1)
            {
                
                searchGridView.Rows.RemoveAt(e.RowIndex);
                searchGridView.Refresh();
                searchStrings.RemoveAt(e.RowIndex);

            }
        }


        private void btnApply_Click(object sender, EventArgs e)
        {
            AppSetting.ThreadCount = cmbThreadCount.SelectedIndex + 1;
        }

        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            CheckForEnableApplyButton();
        }

        private void CheckForEnableApplyButton()
        {
        }

        private void txtMax_TextChanged(object sender, EventArgs e)
        {
            CheckForEnableApplyButton();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (this.threads.Count > 0 && this.threads[0].ThreadState.Equals(System.Threading.ThreadState.Suspended))
            {
                foreach (Thread thread in threads)
                {
                    thread.Resume();
                }
            }
            else
            {
                createAndStartThreads();
            }
            this.btnStop.Enabled = !(this.btnPlay.Enabled = false);
        }
        
        private void createAndStartThreads()
        {
            try
            {
                int threadCount = Convert.ToInt16(cmbThreadCount.Text);
                if(threadCount > users.Count)
                {
                    threadCount = users.Count;
                }
                if(searchStrings.Count == 0)
                {
                    MessageBox.Show("Enter search criterias.");
                    return;
                }
                IList<IList<User> > listofUsers  = new List<IList<User>>();
                for (int i = 0; i < threadCount; i++)
                {
                    listofUsers.Add(new List<User>());
                }
                for (int i = 0; i < users.Count; i++)
                {
                    listofUsers[i % threadCount].Add(users[i]);
                }
                IList<VideoExtractorAndCommenter > extractorAndCommenters = new List<VideoExtractorAndCommenter>();
                for (int i = 0; i < threadCount; i++)
                {
                    IAdapter adapter = new WebAdapter(searchStrings[i % searchStrings.Count], 1);
                    IYouTubeAccountPool pool = new YouTubeAccountPool(listofUsers[i], (int)delayMinute.Value);
                    ICommentRepository commentRepository = new CommentRepository();
                    IVideoCommenter videoCommenter = new VideoCommenter(pool, commentRepository);
                    VideoExtractorAndCommenter videoExtractorAndCommenter= new VideoExtractorAndCommenter(adapter, videoCommenter, (int)minViewCount.Value);
                    extractorAndCommenters.Add(videoExtractorAndCommenter);
                }
                for (int i = threadCount; i < searchStrings.Count; i++)
                {
                    extractorAndCommenters[i % threadCount].addSearchStringPhrase(searchStrings[i]);
                }
                foreach (VideoExtractorAndCommenter commenter in extractorAndCommenters)
                {
                    threads.Add(new System.Threading.Thread(new System.Threading.ThreadStart(commenter.extractVideosAndInsertAComment)) { IsBackground = true });
                }
                for (int i = 0; i < threadCount; i++)
                {
                    threads[i].Start();
                }
                //new System.Threading.Thread(new System.Threading.ThreadStart(this.test)).Start();
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine(ex.Message);
            }
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

            this.Invoke(new MethodInvoker(delegate()
            {
                this.btnStop.Enabled = !(this.btnPlay.Enabled = true);
            }));
            stopThreads();
        }

        private void stopThreads()
        {
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Suspend();
            }
        }

        private void cmbThreadCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckForEnableApplyButton();
        }



        private void btnSave_Click(object sender, EventArgs e)
        {
            AppSetting.MinViewCount = (int)minViewCount.Value;

            AppSetting.DelayMinute = (int)delayMinute.Value;
            MessageBox.Show("Save operation completed successfully.");

            //Classes.AppSetting.MaxF = (int)MaxF.Value;

            //Classes.AppSetting.MinU = (int)MinU.Value;

            //Classes.AppSetting.MaxU = (int)MaxU.Value;
        }

        private void addSearchCriteriaButton_Click(object sender, EventArgs e)
        {
            if (!searchCriteriatextBox.Text.Trim().Equals(""))
            {
                bool isExist = false;
                foreach (string searchString in searchStrings)
                {
                    if (searchCriteriatextBox.Text.ToLower().Trim().Equals(searchString))
                    {
                        isExist = true;
                    }
                }
                if (!isExist)
                {
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Value = searchCriteriatextBox.Text.Trim();
                    searchStrings.Add(cell.Value.ToString());
                    DataGridViewRow row = new DataGridViewRow();
                    row.Cells.Add(cell);
                    cell = new DataGridViewButtonCell();
                    cell.Value = "X";
                    row.Cells.Add(cell);
                    searchGridView.Rows.Add(row);
                    searchCriteriatextBox.Text = "";

                }
            }
        }

        private void saveSearchbutton_Click(object sender, EventArgs e)
        {
            try
            {
                IList<string > list = new List<string>();
                foreach (DataGridViewRow row in this.searchGridView.Rows)
                {
                    list.Add(row.Cells[0].Value.ToString());
                }
                SearchCriteriaStorageManager.resetFileAndSaveList(list);
                MessageBox.Show("Save operation completed successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                
            }
        }

        public void test()
        {
            Console.Write("test");
        }
        private void outputtextBox_TextChanged(object sender, EventArgs e)
        {
            string name = "";
            if (outputtextBox.InvokeRequired)
            {
                outputtextBox.Invoke(new MethodInvoker(delegate {}));
            }

        }

    }
}
