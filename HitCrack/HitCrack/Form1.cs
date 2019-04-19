using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web;

namespace HitCrack
{
    public partial class HitCrack : Form
    {
        public HitCrack()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            aval=txtAval.Text;
            bval = txtBval.Text;
            cpgid = txtGid.Text;
            cpcid = txtCid.Text;
            if ((cpgid == "") || (cpcid == ""))
            {
                MessageBox.Show("参数cpgid cpcid不能为空");
                return;
            }
            GeneratePara();
            //return;
            tt = Convert.ToInt32(txtCnt.Text);
            cnt = 0;
            cfin = 0;
            cwork = 0;
            btnExecute.Enabled = false;
            for (int i = 0; i<100 ; i++)
            {
                Thread th = new Thread(new ThreadStart(WebThread));
                th.IsBackground = true;
                th.Start();
            }
        }

        private void GeneratePara()
        {
            string orn = HttpPost.GetContent("http://" + aval + ".mikecrm.com/" + bval);
            string ctt = orn.ToString();
            string i,s,acc;
            //string cp,cid,cval;
            Regex reg = new Regex("var SOUL = (.+);");
            Match mt=reg.Match(ctt);
            ctt=mt.Groups[1].Value;
            reg = new Regex("\"I\":(\\w*)");
            mt = reg.Match(ctt);
            i = mt.Groups[1].Value;
            reg = new Regex("\"FRS\":(\\w*)");
            mt = reg.Match(ctt);
            s = mt.Groups[1].Value;
            reg = new Regex("\"ACC\":\"(\\w*)\"");
            mt = reg.Match(ctt);
            acc = mt.Groups[1].Value;
            //reg = new Regex("\"cp\":{(.+)},");
            //mt = reg.Match(ctt);
            //cp = mt.Groups[1].Value;
            
            //reg = new Regex("\\[(\\w*),\"1\"\\]");
            //mt = reg.Match(cp);
            //cid=mt.Groups[1].Value;
            //reg = new Regex("\""+cid+"\":(\\w*)");
            //mt = reg.Match(cp);
            //cval = mt.Groups[1].Value; //cp value

            //reg = new Regex("\\$c(\\w*).1.0.0:\\$item0.0.1.0\">1");
            //mt = reg.Match(orn);
            //lblResult.Text = mt.Groups[1].Value;
            dval = "{\"cvs\":{\"i\":" + i + ",\"t\":\"" + bval + "\",\"s\":" + s + ",\"acc\":\"" + acc + "\",\"r\":\"\",\"c\":{\"cp\":{\""+cpgid+"\":"+cpcid+"}}}}";
            dval = HttpUtility.UrlEncode(dval, Encoding.UTF8);
        }
        
        private void WebThread()
        {
            mwork.WaitOne();
            cwork++;
            mwork.ReleaseMutex();
            while (true)
            {
                //是否增加已完成个数
                mutex.WaitOne();
                if (cnt == tt)
                {
                    mwork.WaitOne();
                    cwork--;
                    if(cwork==0)
                        this.btnExecute.BeginInvoke(new MethodInvoker(() => { MessageBox.Show("Complete", "Progress"); btnExecute.Enabled = true; }));
                    mwork.ReleaseMutex();
                    
                    mutex.ReleaseMutex();
                    return;
                }
                cnt++;
                mutex.ReleaseMutex();
                
                try
                {
                    string getstr = HttpPost.Get("http://" + aval + ".mikecrm.com/" + bval);
                    string fdata = "d=" + dval;
                    HttpPost.Post("http://" + aval + ".mikecrm.com/handler/web/form_runtime/handleSubmit.php", fdata, getstr);
                }
                catch (Exception)
                {
                }
                mfin.WaitOne();
                cfin++;
                this.lblResult.BeginInvoke(new MethodInvoker(() => { lblResult.Text = "已经完成任务" + cfin.ToString() + "个" + ",共" + tt.ToString() + "个"; }));
                mfin.ReleaseMutex();
            }
        }
        static int cnt; //载入+完成的任务
        static int cfin; //完成的
        static int cwork; //工作的
        static Mutex mutex = new Mutex();
        static Mutex mfin = new Mutex();
        static Mutex mwork = new Mutex();
        static int tt;
        static string dval="";
        static string aval = "";
        static string bval = "";
        static string cpgid = "";
        static string cpcid = "";

        private void btnFind_Click(object sender, EventArgs e)
        {
            string ctt = HttpPost.GetContent("http://" + txtAval.Text + ".mikecrm.com/" + txtBval.Text);
            Regex reg = new Regex("var SOUL = (.+);");
            Match mt = reg.Match(ctt);
            MatchCollection mtc;
            ctt = mt.Groups[1].Value;

            //获取所有控件组
            reg = new Regex(@"""cpo"":""([0-9;]*)""");
            mt=reg.Match(ctt);
            string ctlg = mt.Groups[1].Value;

            //每组遍历控件
            reg = new Regex(@"([0-9]+)");
            mtc = reg.Matches(ctlg);
            foreach(Match mte in mtc)
            {
                //其中一个控件组
                string strgid = mte.Groups[1].Value;
                reg = new Regex(@"(""" + strgid + @""":\[\d,\d,\{""l"":\[((\[([0-9]*),""([^""]*)""\],)*)(\[([0-9]*),""([^""]*)""\]?)\],""df"":\[\],""t"":""([^""]*)""((,""d"":"""",""ts"":null,""ds"":null)?),""l2s"":\{((""([0-9]*)"":([0-9]*),)*)((""([0-9]*)"":([0-9]*))?)\}\},{""rwh"":\d},\d])");
                mt = reg.Match(ctt);
                string strg=mt.Groups[1].Value;

                //获取标题
                reg = new Regex(@"""t"":""([^""]*)""");
                mt = reg.Match(strg);
                string gti=mt.Groups[1].Value; //标题

                //获取所有符合要求的控件
                reg = new Regex(@"\[([0-9]*),"""+txtOpt.Text+@"""\]");
                mtc=reg.Matches(strg);
                foreach(Match mc in mtc)
                {
                    //其中一个符合要求的控件
                    string scId = mc.Groups[1].Value;
                    reg = new Regex(@""""+scId+@""":([0-9]*)");
                    mt = reg.Match(strg);
                    string cId = mt.Groups[1].Value;
                    DialogResult dres=MessageBox.Show("Find A Result:\nTitle of Control Group:" + gti +"\nClick YES to Confirm,Click NO to Find Next,Click CANCEL to Stop Searching", "Search Result", MessageBoxButtons.YesNoCancel);
                    if (dres == DialogResult.Yes)
                    {
                        //设置文本框的文本
                        txtCid.Text=cId;
                        txtGid.Text=strgid;
                        return;
                    }
                    else if (dres == DialogResult.No)
                    {
                        continue;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }
}
