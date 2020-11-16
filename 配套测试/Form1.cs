using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO.Ports;
using System.IO;
using System.Drawing.Printing;
using System.Text.RegularExpressions;

using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace 配套测试
{
    public partial class Form1 : Form
    {
        int byteRecvSum = 0;
        string strDataRecv = "";
        byte[] byteSerialData = new byte[1024];

        #region   // CRC校验计算用数据

        //public static string[] byteToHexStr = new string[] 
        //                                        { "00 ", "01 ", "02 ", "03 ", "04 ", "05 ", "06 ", "07 ", "08 ", "09 ", "0A ", "0B ", "0C ", "0D ", "0E ", "0F ", 
        //                                          "10 ", "11 ", "12 ", "13 ", "14 ", "15 ", "16 ", "17 ", "18 ", "19 ", "1A ", "1B ", "1C ", "1D ", "1E ", "1F ",
        //                                          "20 ", "21 ", "22 ", "23 ", "24 ", "25 ", "26 ", "27 ", "28 ", "29 ", "2A ", "2B ", "2C ", "2D ", "2E ", "2F ",
        //                                          "30 ", "31 ", "32 ", "33 ", "34 ", "35 ", "36 ", "37 ", "38 ", "39 ", "3A ", "3B ", "3C ", "3D ", "3E ", "3F ",
        //                                          "40 ", "41 ", "42 ", "43 ", "44 ", "45 ", "46 ", "47 ", "48 ", "49 ", "4A ", "4B ", "4C ", "4D ", "4E ", "4F ",
        //                                          "50 ", "51 ", "52 ", "53 ", "54 ", "55 ", "56 ", "57 ", "58 ", "59 ", "5A ", "5B ", "5C ", "5D ", "5E ", "5F ",
        //                                          "60 ", "61 ", "62 ", "63 ", "64 ", "65 ", "66 ", "67 ", "68 ", "69 ", "6A ", "6B ", "6C ", "6D ", "6E ", "6F ",
        //                                          "70 ", "71 ", "72 ", "73 ", "74 ", "75 ", "76 ", "77 ", "78 ", "79 ", "7A ", "7B ", "7C ", "7D ", "7E ", "7F ",
        //                                          "80 ", "81 ", "82 ", "83 ", "84 ", "85 ", "86 ", "87 ", "88 ", "89 ", "8A ", "8B ", "8C ", "8D ", "8E ", "8F ",
        //                                          "90 ", "91 ", "92 ", "93 ", "94 ", "95 ", "96 ", "97 ", "98 ", "99 ", "9A ", "9B ", "9C ", "9D ", "9E ", "9F ",
        //                                          "A0 ", "A1 ", "A2 ", "A3 ", "A4 ", "A5 ", "A6 ", "A7 ", "A8 ", "A9 ", "AA ", "AB ", "AC ", "AD ", "AE ", "AF ",
        //                                          "B0 ", "B1 ", "B2 ", "B3 ", "B4 ", "B5 ", "B6 ", "B7 ", "B8 ", "B9 ", "BA ", "BB ", "BC ", "BD ", "BE ", "BF ",
        //                                          "C0 ", "C1 ", "C2 ", "C3 ", "C4 ", "C5 ", "C6 ", "C7 ", "C8 ", "C9 ", "CA ", "CB ", "CC ", "CD ", "CE ", "CF ",
        //                                          "D0 ", "D1 ", "D2 ", "D3 ", "D4 ", "D5 ", "D6 ", "D7 ", "D8 ", "D9 ", "DA ", "DB ", "DC ", "DD ", "DE ", "DF ",
        //                                          "E0 ", "E1 ", "E2 ", "E3 ", "E4 ", "E5 ", "E6 ", "E7 ", "E8 ", "E9 ", "EA ", "EB ", "EC ", "ED ", "EE ", "EF ",
        //                                          "F0 ", "F1 ", "F2 ", "F3 ", "F4 ", "F5 ", "F6 ", "F7 ", "F8 ", "F9 ", "FA ", "FB ", "FC ", "FD ", "FE ", "FF "};

        public char[] NumToHex = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        //////////////* CRC 计算高8位*///////////////////
        readonly static byte[] ResultCRCHigh = new byte[] {
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
        0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
        0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81,
        0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
        0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
        0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
        0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
        0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
        0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
        0x40
        };

        //////////////*CRC计算低8位*///////////////////
        readonly static byte[] ResultCRCLow = new byte[] {
        0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4,
        0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
        0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD,
        0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
        0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7,
        0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
        0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE,
        0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
        0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2,
        0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
        0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB,
        0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
        0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91,
        0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
        0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88,
        0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
        0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80,
        0x40
        };
        #endregion




        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenClose_Click(object sender, EventArgs e)
        {
            if (btnOpenClose.Text == "打开")
            {
                if (serialPort1.IsOpen)
                {
                    //串口已经打开了，将其关闭
                    try
                    {
                        serialPort1.Close();
                        btnOpenClose.Text = "关闭";
                        btnOpenClose.BackColor = Color.WhiteSmoke;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {

                    }
                }
            } else if ( btnOpenClose.Text == "关闭")
            {
                //有可用的串口， 将串口号赋值给serial
                serialPort1.Close();    //更改串口号前， 先关闭串口
                serialPort1.PortName = comboBoxPort.Text;
                serialPort1.BaudRate = 9600;
                serialPort1.DataBits = 8;
                serialPort1.Parity = (Parity)(0);
                serialPort1.StopBits = (StopBits)(1);


                // 要打开串口
                if (!serialPort1.IsOpen)
                {
                    //串口未打开， 下面打开串口
                    try
                    {
                        serialPort1.Open();
                        btnOpenClose.Text = "打开";
                        btnOpenClose.BackColor = Color.GreenYellow;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {

                    }
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //查询当前可用串口号
            string[] portStr = SerialPort.GetPortNames();

            comboBoxPort.Items.Clear();
            comboBoxPort.Text = "";

            foreach (string str in portStr)
            {
                comboBoxPort.Items.Add(str);
            }

            if (portStr.Length > 0)
            {
                comboBoxPort.Text = portStr[0];
            }
        }

        private byte HexCharToValue(char cInput)
        {
            byte byteValue = 0;
            if (cInput >= 0x30 && cInput <= 0x39)
            {
                byteValue = (byte)(cInput - 0x30);
            }
            if (cInput >= 0x41 && cInput <= 0x46)
            {
                byteValue = (byte)(cInput - 0x41 + 10);
            }
            if (cInput >= 0x61 && cInput <= 0x66)
            {
                byteValue = (byte)(cInput - 0x61 + 10);
            }


            return byteValue;
        }

        private byte[] strToHex(string strInput)  //将string字符串，以byte形式发送
        {
            byte[] byteToReturn = new byte[1000];
            int j = 0;

            for (int i = 0; i < strInput.Length; i++)
            {
                if (i % 3 == 2) //如果不是空格分隔hex数据，就报错，不发送数据
                {
                    if (strInput[i] != 0x20)
                    {
                        MessageBox.Show("输入的16进制数据，空格位置不对，发送失败！");
                        return null;
                    }
                }
            }


            byte temp = 0;
            for (int i = 0; i < strInput.Length; i += 3)
            {

                if ((i + 1) < strInput.Length)
                {
                    temp = (byte)(HexCharToValue(strInput[i]) * 16);
                    temp += HexCharToValue(strInput[i + 1]);
                }
                else
                {
                    temp = HexCharToValue(strInput[i]);
                }

                byteToReturn[j++] = temp;
            }
            if (serialPort1.IsOpen)
            {
                serialPort1.Write(byteToReturn, 0, j);
            }
            else
            {
                MessageBox.Show("请先打开串口！");
            }


            return byteToReturn;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            strToHex(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            strToHex(textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            strToHex(textBox3.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            strToHex(textBox4.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byteRecvSum = serialPort1.BytesToRead;

            if (byteRecvSum >= 1024)
            {  //超过了数组下标了
                return;
            }


            serialPort1.Read(byteSerialData, 0, byteRecvSum);
            serialPort1.ReadExisting();  //将串口接收的数据读空

            strDataRecv += GetString(byteSerialData, byteRecvSum);  //转换成10进制的， 字符串数据
        }

        private string GetString(byte[] bytes, int length)
        {  // 转换程10进制的 字符串数据
            string strData = "";
            for (int i = 0; i < length; i++)
            {
                strData += ((bytes[i]).ToString()) + " ";

            }
            return strData;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateMotorStateAndInquiry();
        }

        //查询并更改状态， 并作对应的应答
        private void updateMotorStateAndInquiry()
        {
            //if (isDataImport == false)
            //{
            //    return;
            //}

            int CRCEnd;
            //int iCRCHeadeIndex = 0, iCRCTailIndex = 0;
            int iCRCResult = 0;


            byte[] byteSendData = new byte[64];

            #region  //根据代码是否被接收， 进行不同数据发送
            //if (IsDataReach == true)              // 长度和高度已送达，发送查询的命令
            //{
            //    byteSendData[0] = 0x01;  //从机地址0x01；
            //    byteSendData[1] = 0x04;  //功能码，读取寄存器
            //    byteSendData[2] = 0x00;  //寄存器起始地址，高8bit
            //    byteSendData[3] = 0x12;  //寄存器起始地址，低8bit
            //    byteSendData[4] = 0x00;  //读取寄存器数量，高8bit
            //    byteSendData[5] = 0x02;  //读取寄存器数量，低8bit
            //    CRCEnd = crc16(GetCertainData(byteSendData, (int)0, (int)6));
            //    byteSendData[6] = (byte)((CRCEnd & 0xFF00) >> 8);
            //    byteSendData[7] = (byte)(CRCEnd & 0xFF);
            //    if (serialPort1.IsOpen)
            //    {
            //        serialPort1.Write(byteSendData, 0, 8);
            //    }
            //}
            //else if (IsDataReach == false)
            //{  // 重复发送数据，直到被接收
            //    //if (byteCurData[0] == 0)
            //    //{
            //    //    byteCurData[0] = 1;  //byte[0]标识数据编号， 值为0标识，还未开始。
            //    //}
            //    byteSendData[0] = 0x01;  //从机地址0x01；
            //    byteSendData[1] = 0x1F;  //功能码，自定义数据，发送切割角度、长度、高度
            //    byteSendData[2] = 0x00;  //寄存器起始地址，高8bit
            //    byteSendData[3] = 0x20;  //寄存器起始地址，低8bit
            //    byteSendData[4] = 0x00;  //写入数据个数，高8bit
            //    byteSendData[5] = 0x06;  //写入寄存器数量，低8bit

            //    //byteSendData[6] = 0;   // 等于0 表明 不管上一次长度是否一致，一定重发本次数据

            //    byteSendData[6] = byteCurData[0];   // 数据发送的次数， 预留的， 实际未使用

            //    byteSendData[7] = byteCurData[1];  // 切割角度，4是45/45， 3是90/45， 2是45/90， 1是90/90
            //    byteSendData[8] = byteCurData[2];  // 切割长度×10后的高8bit
            //    byteSendData[9] = byteCurData[3];  // 切割长度×10后的低8bit
            //    byteSendData[10] = byteCurData[4]; // 切割高度×10后的高8bit 
            //    byteSendData[11] = byteCurData[5]; // 切割高度×10后的低8bit

            //    CRCEnd = crc16(GetCertainData(byteSendData, (int)0, (int)12));
            //    byteSendData[12] = (byte)((CRCEnd & 0xFF00) >> 8);
            //    byteSendData[13] = (byte)(CRCEnd & 0xFF);
            //    if (serialPort1.IsOpen)
            //    {
            //        serialPort1.Write(byteSendData, 0, 14);
            //    }

            //}
            #endregion

            #region  // 解析接收到的数据，判断是否有有效数据接收到
            if (!String.IsNullOrEmpty(strDataRecv)) //字符串为空，说明无数据
            {
                byte[] bytesRecv = GetBytes(strDataRecv);

                //校验数据前，先把键盘按钮变成白色
                //btnSeting.BackColor = Color.WhiteSmoke;
                ////判断是否有校验合格的modbus数据帧
                for (int i = 0; i < bytesRecv.Length - 2; i++)
                {
                    if (bytesRecv[i] == 0x01)
                    {
                        if (bytesRecv[i + 1] > 0x00 && bytesRecv[i + 1] < 24 || bytesRecv[i + 1] == 0x1F)
                        {
                            //帧数据地址和 功能码都在合理范围内
                            for (int j = 2; j < bytesRecv.Length - 2 - i; j++)
                            {
                                iCRCResult = (bytesRecv[i + j + 1] << 8) | bytesRecv[i + j + 2];

                                if (iCRCResult == crc16(GetCertainData(bytesRecv, i, j + 1)))
                                {
                                    RespondModbusData(bytesRecv, i, j + 1);
                                    // 0915：用于获取要截取的字符串index
                                    int iStartIndex = MatchNTimes(strDataRecv, ' ', i + j + 1);
                                    if (iStartIndex != -1)
                                    {
                                        strDataRecv = strDataRecv.Substring(iStartIndex + 1);
                                    }

                                    //btnSeting.BackColor = Color.GreenYellow;
                                }

                            }
                        }
                    }
                }
            }
            #endregion

            

        }

        private byte[] GetBytes(string strData)  // string 装换成byte[], string中相邻的byte 用 空格 隔开
        {

            if (string.IsNullOrEmpty(strData))
            {
                return new byte[] { 0xFF };  //输入的字符串有问题
            }
            string[] byteStrArray = Regex.Split(strData, " ", RegexOptions.IgnoreCase);

            byte[] bytes = new byte[byteStrArray.Length - 1];

            for (int i = 0; i < byteStrArray.Length - 1; i++)  //因为最后一个空格后是空的字符串
            {
                if (!string.IsNullOrEmpty(byteStrArray[i]))
                {
                    bytes[i] = Convert.ToByte(byteStrArray[i]);
                }
            }
            return bytes;
        }

        //进行crc校验计算
        private int crc16(byte[] byteCRC)
        {
            int uchCRCHi = 0xFF;
            int uchCRCLo = 0xFF;
            int uIndex;

            for (int i = 0; i < byteCRC.Length; i++)
            {
                uIndex = uchCRCHi ^ ((int)byteCRC[i]);
                uchCRCHi = uchCRCLo ^ ResultCRCHigh[uIndex];
                uchCRCLo = ResultCRCLow[uIndex];
            }
            return (((uchCRCHi) << 8) | uchCRCLo);


        }

        // 截取制定的下标byte数组
        private byte[] GetCertainData(byte[] byteData, int iOffset, int iSum)
        {
            byte[] byteReturnData = new byte[iSum];

            for (int i = 0; i < iSum; i++)
            {
                byteReturnData[i] = byteData[iOffset + i];
            }

            return byteReturnData;
        }

        //将有效的modbus数据， 进行解析，并作出回应
        private void RespondModbusData(byte[] ModbusData, int iOffset, int iSum)
        {
            byte[] ByteSendData = new byte[256];
            int CRCEnd = 0xFFFF;
            int byteSum = 0; ;

            //定时器中已经检查目标地址为0x01， 所以不再做判断
            if (ModbusData[iOffset + 1] == 0x01)  //功能码0x01
            {   //功能码是0x01， 即读取线圈
                //int modbusAddr = (ModbusData[iOffset + 2] << 8) | ModbusData[iOffset + 3];
                //if (modbusAddr >= 0 && modbusAddr < 160)
                //{
                //    ByteSendData[0] = ModbusData[0];
                //    ByteSendData[1] = ModbusData[1];
                //    ByteSendData[2] = 2;    //有16位， 即2个字节的线圈
                //    ByteSendData[3] = (byte)(iCoilState & 0xFF);
                //    ByteSendData[4] = (byte)((iCoilState & 0xFF00) >> 8);
                //    CRCEnd = crc16(GetCertainData(ByteSendData, 0, 5));
                //    ByteSendData[5] = (byte)((CRCEnd & 0xFF00) >> 8);
                //    ByteSendData[6] = (byte)(CRCEnd & 0xFF);
                //    ByteSendData[7] = 0x0D;
                //    ByteSendData[8] = 0x0A;
                //    if (serialPort1.IsOpen)
                //    {
                //        serialPort1.Write(ByteSendData, 0, 9);
                //    }
                //}

            }
            else if (ModbusData[iOffset + 1] == 0x04)
            {   //功能查询的返回值， 接收到来自单片机的应答
                //byteSum = ModbusData[iOffset + 2];  //获取字节数  

                //if (byteSum == 4)  // 读取从0x12起的，两个寄存器
                //{
                //    DataIndicate[12] = (UInt16)((((Int16)ModbusData[iOffset + 3]) << 8) + ModbusData[iOffset + 4]);  //对应的是机器的状态
                //    DataIndicate[13] = (UInt16)((((Int16)ModbusData[iOffset + 5]) << 8) + ModbusData[iOffset + 6]);
                //}
                //else
                //{
                //    return;
                //}
            }
            else if (ModbusData[iOffset + 1] == 0x1F)  //功能码0x1F， 自定义发送完整长度和高度的功能码， 确定接受到了应答
            {  // 如果height为0说明不用发高度，所以不验证高度
                //if (ModbusData[iOffset + 2] == byteCurData[0] && ModbusData[iOffset + 3] == byteCurData[1] &&
                //    ModbusData[iOffset + 4] == byteCurData[2] && ModbusData[iOffset + 5] == byteCurData[3]) //判断长度、角度、高度一致，说明接收到了数据
                //{
                //    byteCurData[0]++;
                //    IsDataReach = true;
                //}

                ByteSendData[0] = 0x01;  //从机地址0x01；
                ByteSendData[1] = 0x1F;  //功能码，自定义数据，发送切割角度、长度、高度
                ByteSendData[2] = ModbusData[iOffset+6];   // 数据发送的次数， 预留的， 实际未使用
                ByteSendData[3] = ModbusData[iOffset + 7];  // 切割角度，4是45/45， 3是90/45， 2是45/90， 1是90/90
                ByteSendData[4] = ModbusData[iOffset + 8];  // 切割长度×10后的高8bit
                ByteSendData[5] = ModbusData[iOffset + 9];  // 切割长度×10后的低8bit
                ByteSendData[6] = ModbusData[iOffset + 10]; // 切割高度×10后的高8bit 
                ByteSendData[7] = ModbusData[iOffset + 11]; // 切割高度×10后的低8bit

                CRCEnd = crc16(GetCertainData(ByteSendData, (int)0, (int)8));
                ByteSendData[8] = (byte)((CRCEnd & 0xFF00) >> 8);
                ByteSendData[9] = (byte)(CRCEnd & 0xFF);
                if (serialPort1.IsOpen)
                {
                    serialPort1.Write(ByteSendData, 0, 10);
                }

                #region  // #更新切割角度

                switch (ModbusData[iOffset + 7])  // 更新切割角度到 通讯数据缓存种
                {
                    case 1:
                        button5.Text = "90-90";
                        break;
                    case 2:
                        button5.Text = "45-90";
                        break;
                    case 3:
                        button5.Text = "90-45";
                        break;
                    case 4:
                        button5.Text = "45-45";
                        break;
                    default:
                        //MessageBox.Show("数据表的切割角度不正确");
                        return;
                    // break;
                }
                #endregion

                double dLength = (double)(ModbusData[iOffset + 8] * 256 + ModbusData[iOffset + 9]) / 10;
                double dHeight = (double)(ModbusData[iOffset + 10] * 256 + ModbusData[iOffset + 11]) / 10;
                tbLength.Text = dLength.ToString();
                tbHeight.Text = dHeight.ToString();


            }

        }

        // 返回某个字符，在字符串中第n次出现的位置
        // strData被搜索的字符串， cToLookFor：被搜寻的字符， times：出现的次数
        int MatchNTimes(string strData, char cToLookFor, int times)
        {
            int i;

            if (string.IsNullOrEmpty(strData))
            {
                return -1;
            }

            int charCount = 0;
            for (i = 0; i < strData.Length; i++)
            {
                if (cToLookFor == strData[i])
                {
                    charCount++;
                }
            }

            if (times > charCount)
            {
                return -1;
            }

            int index = 0;
            for (i = 1; i < charCount; i++)
            {
                index = strData.IndexOf(cToLookFor, index);
                if (i == times)
                {
                    return index;
                }
                else
                {
                    index = strData.IndexOf(cToLookFor, index + 1);
                }
            }

            return -1;
        }



    }
}
