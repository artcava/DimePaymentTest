using DimeXplorer;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DimePaymentTest
{
    public partial class frmMain : Form
    {
        private bool dimeOk;
        delegate void SetCmdTextCallback();
        delegate void SetLabelTextCallback();
        delegate void SetSearchingTextCallback();
        private Thread c = null;
        private Thread t = null;

        public frmMain()
        {
            InitializeComponent();
            c = new Thread(new ThreadStart(CheckDimeOk));
            c.Start();
            c.IsBackground = true;
            t = new Thread(new ThreadStart(CheckPayment));
            t.Start();
            t.IsBackground = true;
        }

        private void cmdRoll_Click(object sender, EventArgs e)
        {
            Form frm = null;
            if (!dimeOk)
                frm = new frmTransaction();
            else
                frm = new frmYouWin();
            frm.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void CheckPayment()
        {
            while (!dimeOk)
            {
                var walletPrivate = Wallet.GetPrivateWallet();
                if (walletPrivate == null) continue;

                var walletPublic = Wallet.GetPublicWallet();
                if (walletPublic == null) continue;

                var explorer = new Explorer();
                var txList = explorer.getTransactionsByAddress(1, 20, walletPublic);
                if (txList != null)
                {
                    foreach (var dTx in txList.Data)
                    {
                        var tx = explorer.getTransaction(dTx.TransactionHash);
                        if (tx == null) continue;
                        var wPub = tx.AddressOutputs.FirstOrDefault(w => w.WalletAddress.Equals(walletPublic));
                        if (wPub == null) break;

                        var wPriv = tx.AddressInputs.FirstOrDefault(w => w.WalletAddress.Equals(walletPrivate));
                        if (wPriv == null) break;

                        dimeOk = (wPub.Value >= 100M);
                    }
                }
            }
        }

        private void CheckDimeOk()
        {
            while (!dimeOk)
            {
                SetSearchingText();
                Thread.Sleep(5000);
            }

            if (dimeOk)
            {
                SetCmdText();
                SetLabelText();
                SetSearchingText();
                //cmdRoll.Text = "Roll";
                //label2.Text = "Click Roll to play";
            }
        }

        private void SetSearchingText()
        {
            if (label3.InvokeRequired)
            {
                SetSearchingTextCallback t = new SetSearchingTextCallback(SetSearchingText);
                Invoke(t);
            }
            else
                label3.Text = (dimeOk) ? "" : "Scanning Blockchain for your payment...";
        }

        private void SetCmdText()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (cmdRoll.InvokeRequired)
            {
                SetCmdTextCallback d = new SetCmdTextCallback(SetCmdText);
                Invoke(d);
            }
            else
            {
                cmdRoll.Text = "Roll";
            }
        }

        private void SetLabelText()
        {
            if (label2.InvokeRequired)
            {
                SetLabelTextCallback d = new SetLabelTextCallback(SetLabelText);
                Invoke(d);
            }
            else
            {
                label2.Text = "Click Roll to play";
            }
        }
    }
}