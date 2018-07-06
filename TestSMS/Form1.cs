using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JamaaTech;
using JamaaTech.Smpp.Net.Client;
using JamaaTech.Smpp.Net.Lib;
using JamaaTech.Smpp.Net.Lib.Protocol;
using JamaaTech.Smpp.Net.Lib.Util;

namespace TestSMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SmppClientSession mmSession;
        SubmitSm mmSubmitSm;
        SubmitSmResp mmSubmitSmResp;
        SessionBindInfo mmSessionBindInfo;
        Unbind mmUnbind;
        UnbindResp mmUnbindResp;
        Udh mmUdh;


        private void button1_Click(object sender, EventArgs e)
        {
                SMPPEncodingUtil.UCS2Encoding = Encoding.BigEndianUnicode;
            
                mmSessionBindInfo = new SessionBindInfo(); 
                mmSessionBindInfo.SystemID = "********";
                mmSessionBindInfo.Password = "********";
                mmSessionBindInfo.ServerName = "*******"; 
                mmSessionBindInfo.Port = 1234; 
                mmSessionBindInfo.InterfaceVersion = InterfaceVersion.v34; 
                mmSessionBindInfo.SystemType = "SMPP"; 
                mmSessionBindInfo.AllowTransmit = true;
                mmSessionBindInfo.AllowReceive = true; 
                mmSessionBindInfo.AddressNpi = NumberingPlanIndicator.Unknown; 
                mmSessionBindInfo.AddressTon = TypeOfNumber.Unknown;
                mmSession = SmppClientSession.Bind(mmSessionBindInfo, 0); 
                mmSession.PduReceived += new EventHandler<PduReceivedEventArgs>(PduReceivedEventHander);

                
                mmSubmitSm = new SubmitSm();

                mmSubmitSm.DataCoding = DataCoding.UCS2;
                mmSubmitSm.ServiceType = "CMT"; // cellular  messaging
                mmSubmitSm.RegisteredDelivery = RegisteredDelivery.DeliveryReceipt;
                mmSubmitSm.DestinationAddress.Address = "********"; // destination
                mmSubmitSm.DestinationAddress.Npi = NumberingPlanIndicator.ISDN; 
                mmSubmitSm.DestinationAddress.Ton = TypeOfNumber.International; 
                mmSubmitSm.SourceAddress.Npi = NumberingPlanIndicator.Unknown;
                mmSubmitSm.SourceAddress.Ton = TypeOfNumber.Aphanumeric; 
                mmSubmitSm.SourceAddress.Address = "*********";

                try
                {
                    mmSubmitSm.SetMessageText(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("Текст")), DataCoding.UCS2);
                    mmSubmitSmResp = mmSession.SendPdu(mmSubmitSm) as SubmitSmResp; 
                    MessageBox.Show(DateTime.Now.ToString());
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

                MessageBox.Show(mmSubmitSmResp.MessageID + " , " + mmSubmitSmResp.Header.ErrorCode.ToString() + " , " + mmSubmitSm.GetMessageText());
              
        }

       private void PduReceivedEventHander(object sender, PduReceivedEventArgs e)
       {
            MessageBox.Show("Сработало событие received");
            SingleDestinationPDU pdu = e.Request as SingleDestinationPDU;
            if (pdu == null) 
            { 
                return; 
            } 

            Udh udh = null; //We need to test if the UDH field is present
            string message = null; //This will hold the message text
            pdu.GetMessageText(out message, out udh); //Get message text and UDH from the PDU
            MessageBox.Show("Пришедшее на номер: "+message+" , "+pdu.Header.ErrorCode.ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
