using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using Ferryman.DATA;
using Ferryman.Utility;

namespace iosh
{
    public class QuestionClass
    {
        DBClass _DB;
        string strSQL = "";
        DataTable myData = null;
        DataRow[] myRows = null;
        string _ControlName = "";

        public QuestionClass(ref DBClass DB)
        {
            _DB = DB;
        }

        public string ControlName()
        {
            return _ControlName;
        }

        /// <summary>
        /// QuestionType=1
        /// 標題文字
        /// </summary>
        /// <param name="QuestionID"></param>
        /// <param name="QTitle"></param>
        /// <param name="QDescription"></param>
        /// <returns></returns>
        public Literal Create_TitleText(Int64 QuestionID, string QTitle, string QDescription)
        {
            Literal objControl = new Literal();
            objControl.Text = "<h3 class=\"bg-primary\">" + QTitle + "</h3>";
            if (QDescription.Trim()!="")
            {
                objControl.Text += "<br/><p class=\"text-info\">" + HttpUtility.HtmlDecode(QDescription) + "</p>";
            }
            return objControl;
        }

        /// <summary>
        /// QuestionType=2
        /// 文字
        /// </summary>
        /// <param name="QuestionID"></param>
        /// <param name="QTitle"></param>
        /// <param name="QDescription"></param>
        /// <returns></returns>
        public Literal Create_NormalText(Int64 QuestionID, string QTitle, string QDescription)
        {
            Literal objControl = new Literal();
            objControl.Text = "<p class=\"text-primary bg-info\" style=\"margin-bottom: 0px;\">" + QTitle + "</p>";
            if (QDescription.Trim() != "")
            {
                objControl.Text += "<p class=\"text-info\">" + HttpUtility.HtmlDecode(QDescription) + "</p>";
            }
            return objControl;
        }

        /// <summary>
        /// QuestionType=3
        /// 圖片
        /// </summary>
        /// <param name="QuestionID"></param>
        /// <param name="QTitle"></param>
        /// <param name="QDescription"></param>
        /// <returns></returns>
        public Table Create_Image(Int64 QuestionID, string QTitle, string QDescription)
        {
            Table objTable = new Table();
            TableRow objRow;
            TableCell objCell;
            Label objLabel = new Label();
            Image objImage = new Image();

            try
            {
                objTable.ID = "fm-QTable-" + QuestionID.ToString();
                objTable.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                objTable.Width = Unit.Percentage(100);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.CssClass = "bg-info";
                objCell.Text = QTitle;
                if (QDescription != "")
                {
                    objCell.Text += "<br/><small>" + QDescription + "</small>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.Style.Add("padding-left", "30px");

                objImage.ID = "fm_QControl_" + QuestionID.ToString();
                objImage.ClientIDMode = System.Web.UI.ClientIDMode.Static;

                //if (myRow["ParentID"].ToString() != "0")
                //{
                //    objImage.CssClass = "fmRelative_" + myRow["ParentID"].ToString();
                //    objImage.Attributes.Add("disabled", "disabled");
                //}
                objImage.ImageUrl = "object/images/" + Get_ImageSource(QuestionID);
                objCell.Controls.Add(objImage);
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);
                return objTable;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Create_Image", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// QuestionType=11
        /// 產生文字輸入控制項
        /// </summary>
        /// <param name="myRow"></param>
        /// <returns></returns>
        public Table Create_TextBox(DataRow myRow)
        {
            Table objTable = new Table();
            TableRow objRow;
            TableCell objCell;
            Label objLabel = new Label();
            TextBox objTextbox = new TextBox();
            Int64 QuestionID;
            string QTitle;
            string QDescription;
            string IsOption;

            try
            {
                QuestionID = Convert.ToInt64(myRow["QuestionID"]);
                QTitle = myRow["QTitle"].ToString();
                QDescription = myRow["QDescription"].ToString();
                IsOption = myRow["IsOptional"].ToString();

                objTable.ID = "fm-QTable-" + QuestionID.ToString();
                objTable.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                objTable.Width = Unit.Percentage(100);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.CssClass = "bg-info";
                objCell.Text = QTitle;
                if (QDescription!="")
                {
                    objCell.Text += "<br/><small>" + QDescription + "</small>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.Style.Add("padding-left", "30px");

                myData = Get_Option(QuestionID);

                objTextbox.ID = "fm_QControl_" + QuestionID.ToString();
                objTextbox.ClientIDMode = System.Web.UI.ClientIDMode.Static;

                if (myRow["ParentID"].ToString()!="0")
                {
                    objTextbox.CssClass = "fmRelative_" + myRow["ParentID"].ToString();
                    objTextbox.Attributes.Add("disabled", "disabled");
                }

                objCell.Controls.Add(objTextbox);
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);
                return objTable;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Create_TextBox", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// QuestionType=17
        /// 產生量尺輸入控制項
        /// 不使用
        /// </summary>
        /// <param name="myRow"></param>
        /// <returns></returns>
        public Table Create_Slider(DataRow myRow)
        {
            Table objTable = new Table();
            TableRow objRow;
            TableCell objCell;
            Label objLabel = new Label();
            TextBox objTextbox = new TextBox();
            Int64 QuestionID;
            string QTitle;
            string QDescription;
            string IsOption;

            try
            {
                QuestionID = Convert.ToInt64(myRow["QuestionID"]);
                QTitle = myRow["QTitle"].ToString();
                QDescription = myRow["QDescription"].ToString();
                IsOption = myRow["IsOptional"].ToString();

                objTable.ID = "fm-QTable-" + QuestionID.ToString();
                objTable.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                objTable.Width = Unit.Percentage(100);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.CssClass = "bg-info";
                objCell.Text = QTitle;
                if (QDescription != "")
                {
                    objCell.Text += "<br/><small>" + QDescription + "</small>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.Height = Unit.Pixel(80);
                objCell.Style.Add("padding-left", "30px");

                myData = Get_Option(QuestionID);
                if (myData != null)
                {
                    objTextbox.ID = "fm_QControl_" + QuestionID.ToString();
                    objTextbox.ClientIDMode = System.Web.UI.ClientIDMode.Static;

                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        objTextbox.Attributes.Add(myData.Rows[i]["SelectOption"].ToString(), myData.Rows[i]["SelectValue"].ToString());
                    }
                    //if (IsOption == "N")
                    //{
                    //    objTextbox.CssClass = "element-required";
                    //}
                    objCell.Controls.Add(objTextbox);
                }
                else
                {
                    objCell.Text = "<p class=\"text-warning\">問卷選項載入失敗。</p>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);
                return objTable;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Create_TextBox", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// QuestionType=18
        /// 產生資料清單控制項
        /// 不使用
        /// </summary>
        /// <param name="QuestionID"></param>
        /// <param name="QTitle"></param>
        /// <param name="QDescription"></param>
        /// <returns></returns>
        public Table Create_Datalist(Int64 QuestionID, string QTitle, string QDescription)
        {
            Table objTable = new Table();
            TableRow objRow;
            TableCell objCell;
            Label objLabel = new Label();
            TextBox objTextbox = new TextBox();

            try
            {
                objTable.ID = "fm-QTable-" + QuestionID.ToString();
                objTable.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                objTable.Width = Unit.Percentage(100);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.Text = QTitle;
                if (QDescription != "")
                {
                    objCell.Text += "<br/><small>" + QDescription + "</small>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.Style.Add("padding-left", "30px");

                myData = Get_Option(QuestionID);
                if (myData != null)
                {
                    objTextbox.ID = "fm_QControl_" + QuestionID.ToString();
                    objTextbox.ClientIDMode = System.Web.UI.ClientIDMode.Static;

                    for (int i = 0; i < myData.Rows.Count; i++)
                    {
                        objTextbox.Attributes.Add(myData.Rows[i]["SelectOption"].ToString(), myData.Rows[i]["SelectValue"].ToString());
                    }
                    objCell.Controls.Add(objTextbox);
                }
                else
                {
                    objCell.Text = "<p class=\"text-warning\">問卷選項載入失敗。</p>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);
                return objTable;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Create_TextBox", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// QuestionType=12
        /// 產生單選輸入控制項
        /// </summary>
        /// <param name="myRow"></param>
        /// <returns></returns>
        public Table Create_Radiobox(DataRow myRow)
        {
            Table objTable = new Table();
            TableRow objRow;
            TableCell objCell;
            Label objLabel = new Label();
            RadioButtonList objRadio = new RadioButtonList();
            Int64 QuestionID;
            string QTitle;
            string QDescription;
            //string IsOption;
            string strRepeatCount = "";
            Literal objLiteral = null;
            TextBox objText = null;

            try
            {
                QuestionID = Convert.ToInt64(myRow["QuestionID"]);
                QTitle = myRow["QTitle"].ToString();
                QDescription = myRow["QDescription"].ToString();
                //IsOption = myRow["IsOptional"].ToString();

                objTable.ID = "fm-QTable-" + QuestionID.ToString();
                objTable.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                objTable.Width = Unit.Percentage(100);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.CssClass = "bg-info";
                if (QTitle=="[OTHER]")
                {
                    // 其他選項
                    objLiteral = new Literal();
                    objLiteral.Text = "其他，";
                    objCell.Controls.Add(objLiteral);

                    objText = new TextBox();
                    objText.ID = "fm_QTitle_" + QuestionID.ToString();
                    objText.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    objText.Attributes.Add("placeholder", "請說明...");
                    objCell.Controls.Add(objText);
                }
                else
                {
                    // 標準選項
                    objCell.Text = QTitle;
                    if (QDescription != "")
                    {
                        objCell.Text += "<br/><small>" + QDescription + "</small>";
                    }
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.Style.Add("padding-left", "30px");

                myData = Get_Option(QuestionID);
                if (myData != null)
                {
                    objRadio.ID = "fm_QControl_" + QuestionID.ToString();
                    objRadio.CssClass = "option-item";
                    objRadio.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    objRadio.DataSource = myData;
                    objRadio.DataTextField = "SelectOption";
                    objRadio.DataValueField = "SelectValue";
                    //objRadio.RepeatDirection = RepeatDirection.Vertical;
                    objRadio.RepeatDirection = RepeatDirection.Horizontal;
                    objRadio.RepeatLayout = RepeatLayout.Flow;
                    strRepeatCount = Get_RepeatItem_Count(QuestionID);
                    if (strRepeatCount == null)
                    {
                        objRadio.RepeatColumns = 5;
                    }
                    else
                    {
                        objRadio.RepeatColumns = Convert.ToInt32(strRepeatCount);
                    }
                    //objRadio.CssClass = "clear_table";
                    objRadio.DataBind();
                    myRows = myData.Select("DefaultItem='Y'");
                    if (myRows.Length>0)
                    {
                        objRadio.SelectedValue = myRows[0]["SelectValue"].ToString();
                    }
                    myRows = myData.Select("FreeItem='Y'");
                    if (myRows.Length > 0)
                    {
                        ListItem myItem;
                        for (int i=0; i<myData.Rows.Count; i++)
                        {
                            myItem = objRadio.Items.FindByValue(myData.Rows[i]["SelectValue"].ToString());
                            if (myData.Rows[i]["FreeItem"].ToString() == "Y")
                            {
                                myItem.Attributes.Add("onclick", "javascript:relative_check(\"fmRelative_" + QuestionID.ToString() + "\", true);");
                            }
                            else
                            {
                                myItem.Attributes.Add("onclick", "javascript:relative_check(\"fmRelative_" + QuestionID.ToString() + "\", false);");
                            }
                        }
                        //objRadio.Attributes.Add("onchange", "javascript: relative_check(this, \"" + myRows[0]["SelectValue"].ToString() + "\", \"fmRelative_" + QuestionID.ToString() + "\");");
                    }
                    //if (IsOption == "N")
                    //{
                    //    objRadio.CssClass = "element-required";
                    //    objRadio.CssClass = "easyui-validatebox";
                    //    objRadio.Attributes.Add("data-options", "required: true");
                    //}
                    objCell.Controls.Add(objRadio);
                }
                else
                {
                    objCell.Text = "<p class=\"text-warning\">問卷選項載入失敗。</p>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);
                return objTable;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Create_Radiobox", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// QuestionType=13
        /// 產生多選輸入控制項
        /// </summary>
        /// <param name="myRow"></param>
        /// <returns></returns>
        public Table Create_Checkbox(DataRow myRow)
        {
            Literal objQuestion = null;
            Table objTable = new Table();
            TableRow objRow;
            TableCell objCell;
            Label objLabel = new Label();
            //CheckBoxList objCheckbox = new CheckBoxList();
            CheckBox objCheck = null;
            Int64 QuestionID;
            string QTitle;
            string QDescription;
            string IsOption;
            string strRepeatCount = "";
            TextBox objText = null;

            try
            {
                QuestionID = Convert.ToInt64(myRow["QuestionID"]);
                QTitle = myRow["QTitle"].ToString();
                QDescription = myRow["QDescription"].ToString();
                IsOption = myRow["IsOptional"].ToString();

                objTable.ID = "fm-QTable-" + QuestionID.ToString();
                objTable.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                objTable.Width = Unit.Percentage(100);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.CssClass = "bg-info";
                objCell.Text = QTitle;
                if (QDescription != "")
                {
                    objCell.Text += "<br/><small>" + QDescription + "</small>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.Style.Add("padding-left", "30px");

                myData = Get_Option(QuestionID);
                if (myData != null)
                {
                    strRepeatCount = Get_RepeatItem_Count(QuestionID);
                    if (strRepeatCount == null)
                    {
                        strRepeatCount = "5";
                    }

                    objQuestion = new Literal();
                    objQuestion.Text = "<div class='row'>";
                    objCell.Controls.Add(objQuestion);
                    for (int i=0; i<myData.Rows.Count; i++)
                    {
                        objQuestion = new Literal();
                        objQuestion.Text = "<div class='col-lg-3 col-md-4 col-sm-6 col-xs-12'>";
                        objCell.Controls.Add(objQuestion);

                        objCheck = new CheckBox();
                        objCheck.ID = "fm_QControl_" + QuestionID.ToString() + "_" + myData.Rows[i]["OptionID"].ToString();
                        objCheck.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        objCheck.CssClass = "fm_QControl_" + QuestionID.ToString();
                        objCheck.Text = myData.Rows[i]["SelectOption"].ToString();
                        if (myData.Rows[i]["DefaultItem"].ToString()=="Y")
                        {
                            objCheck.Checked = true;
                        }
                        if (myData.Rows[i]["FreeItem"].ToString() == "Y")
                        {
                            objText = new TextBox();
                            objText.ID = "fmRelative_" + QuestionID.ToString() + "_" + myData.Rows[i]["OptionID"].ToString();
                            objText.Attributes.Add("disabled", "disabled");
                            objText.ClientIDMode= System.Web.UI.ClientIDMode.Static;
                            objText.Width = Unit.Percentage(50);
                            objCheck.Attributes.Add("onclick", "others_check('fmRelative_" + QuestionID.ToString() + "_" + myData.Rows[i]["OptionID"].ToString() + "', $(this).prop('checked'));");
                            objCell.Controls.Add(objCheck);
                            objCell.Controls.Add(objText);
                        }
                        else
                        {
                            objCell.Controls.Add(objCheck);
                        }

                        objQuestion = new Literal();
                        objQuestion.Text = "</div>";
                        objCell.Controls.Add(objQuestion);
                    }
                    objQuestion = new Literal();
                    objQuestion.Text = "</div>";
                    objCell.Controls.Add(objQuestion);
                    //objCheckbox.ID = "fm_QControl_" + QuestionID.ToString();
                    //objCheckbox.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    //objCheckbox.DataSource = myData;
                    //objCheckbox.DataTextField = "SelectOption";
                    //objCheckbox.DataValueField = "SelectValue";
                    ////objCheckbox.RepeatDirection = RepeatDirection.Vertical;
                    //objCheckbox.RepeatDirection = RepeatDirection.Horizontal;
                    //objCheckbox.RepeatLayout = RepeatLayout.Flow;
                    //strRepeatCount = Get_RepeatItem_Count(QuestionID);
                    //if (strRepeatCount == null)
                    //{
                    //    objCheckbox.RepeatColumns = 5;
                    //}
                    //else
                    //{
                    //    objCheckbox.RepeatColumns = Convert.ToInt32(strRepeatCount);
                    //}
                    ////objCheckbox.CssClass = "clear_table";
                    //objCheckbox.CssClass = "option-item";
                    //objCheckbox.DataBind();
                    //myRows = myData.Select("DefaultItem='Y'");
                    //if (myRows.Length > 0)
                    //{
                    //    for (int i=0; i<myRows.Length; i++)
                    //    {
                    //        objCheckbox.SelectedValue = myRows[i]["SelectValue"].ToString();
                    //    }
                    //}
                    //myRows = myData.Select("FreeItem='Y'");
                    //if (myRows.Length > 0)
                    //{
                    //    ListItem myItem;
                    //    for (int i = 0; i < myData.Rows.Count; i++)
                    //    {
                    //        myItem = objCheckbox.Items.FindByValue(myData.Rows[i]["SelectValue"].ToString());
                    //        if (myData.Rows[i]["FreeItem"].ToString() == "Y")
                    //        {
                    //            myItem.Attributes.Add("onclick", "javascript:relative_check(\"fmRelative_" + QuestionID.ToString() + "\", true);");
                    //        }
                    //        else
                    //        {
                    //            myItem.Attributes.Add("onclick", "javascript:relative_check(\"fmRelative_" + QuestionID.ToString() + "\", false);");
                    //        }
                    //    }
                    //}
                    //objCell.Controls.Add(objCheckbox);
                }
                else
                {
                    objCell.Text = "<p class=\"text-warning\">問卷選項載入失敗。</p>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);
                return objTable;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Create_Checkbox", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// QuestionType=19
        /// 產生母子選項輸入控制項
        /// </summary>
        /// <param name="myRow"></param>
        /// <returns></returns>
        public Table Create_RelatedRadiobox(DataRow myRow)
        {
            Table objTable = new Table();
            TableRow objRow;
            TableCell objCell;
            Label objLabel = new Label();
            RadioButtonList objRadio = new RadioButtonList();
            RadioButtonList objChildRadio = new RadioButtonList();
            Int64 QuestionID;
            string QTitle;
            string QDescription;
            //string IsOption;
            string strRepeatCount = "";
            Literal objLiteral = null;
            TextBox objText = null;
            DataTable dtSource = null;
            int i;
            List<int> lstJump = new List<int>() ;

            try
            {
                QuestionID = Convert.ToInt64(myRow["QuestionID"]);
                QTitle = myRow["QTitle"].ToString();
                QDescription = myRow["QDescription"].ToString();

                objTable.ID = "fm-QTable-" + QuestionID.ToString();
                objTable.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                objTable.Width = Unit.Percentage(100);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.CssClass = "bg-info";
                if (QTitle == "[OTHER]")
                {
                    // 其他選項
                    objLiteral = new Literal();
                    objLiteral.Text = "其他，";
                    objCell.Controls.Add(objLiteral);

                    objText = new TextBox();
                    objText.ID = "fm_QTitle_" + QuestionID.ToString();
                    objText.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    objText.Attributes.Add("placeholder", "請說明...");
                    objCell.Controls.Add(objText);
                }
                else
                {
                    // 標準選項
                    objCell.Text = QTitle;
                    if (QDescription != "")
                    {
                        objCell.Text += "<br/><small>" + QDescription + "</small>";
                    }
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.Style.Add("padding-left", "30px");

                myData = Get_Option(QuestionID);
                dtSource = myData.Clone();
                myRows = myData.Select("ColumnNo=1");
                for (i=0; i<myRows.Length; i++)
                {
                    dtSource.ImportRow(myRows[i]);
                }

                if (dtSource != null)
                {
                    objRadio.ID = "fm_QControl_" + QuestionID.ToString();
                    objRadio.CssClass = "option-item";
                    objRadio.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    objRadio.DataSource = dtSource;
                    objRadio.DataTextField = "SelectOption";
                    objRadio.DataValueField = "SelectValue";
                    //objRadio.RepeatDirection = RepeatDirection.Vertical;
                    objRadio.RepeatDirection = RepeatDirection.Horizontal;
                    objRadio.RepeatLayout = RepeatLayout.Flow;
                    strRepeatCount = Get_RepeatItem_Count(QuestionID);
                    if (strRepeatCount == null)
                    {
                        objRadio.RepeatColumns = 5;
                    }
                    else
                    {
                        objRadio.RepeatColumns = Convert.ToInt32(strRepeatCount);
                    }
                    objRadio.DataBind();
                    myRows = dtSource.Select("DefaultItem='Y'");
                    if (myRows.Length > 0)
                    {
                        objRadio.SelectedValue = myRows[0]["SelectValue"].ToString();
                    }

                    ListItem myItem;
                    for (i = 0; i < dtSource.Rows.Count; i++)
                    {
                        myItem = objRadio.Items.FindByValue(dtSource.Rows[i]["SelectValue"].ToString());
                        if (Convert.ToInt64(dtSource.Rows[i]["JumpQID"].ToString()) == 0)
                        {
                            // 沒有子選項
                            myItem.Attributes.Add("onclick", "javascript:show_related(\"child-option-" + QuestionID.ToString() + "\", false);");
                        }
                        else
                        {
                            // 有子選項
                            myItem.Attributes.Add("onclick", "javascript:show_related(\"child-option-" + QuestionID.ToString() + "\", true);");
                        }
                    }
                    objCell.Controls.Add(objRadio);

                    objLiteral = new Literal();
                    objLiteral.Text = "<br/>";
                    objCell.Controls.Add(objLiteral);

                    // 產生子選項
                    myRows = myData.Select("ColumnNo=2");
                    dtSource.Clear();
                    for (i = 0; i < myRows.Length; i++)
                    {
                        dtSource.ImportRow(myRows[i]);
                    }

                    if (dtSource != null)
                    {
                        objRadio = new RadioButtonList();
                        objRadio.ID = "fm_QControl_" + QuestionID.ToString() + "_Sub_2";
                        objRadio.CssClass = "option-item child-option-" + QuestionID.ToString();
                        objRadio.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        objRadio.DataSource = dtSource;
                        objRadio.DataTextField = "SelectOption";
                        objRadio.DataValueField = "SelectValue";
                        //objRadio.RepeatDirection = RepeatDirection.Vertical;
                        objRadio.RepeatDirection = RepeatDirection.Horizontal;
                        objRadio.RepeatLayout = RepeatLayout.Flow;
                        strRepeatCount = Get_RepeatItem_Count(QuestionID);
                        if (strRepeatCount == null)
                        {
                            objRadio.RepeatColumns = 5;
                        }
                        else
                        {
                            objRadio.RepeatColumns = Convert.ToInt32(strRepeatCount);
                        }
                        objRadio.Attributes.Add("style", "display:none");
                        objRadio.DataBind();
                        objCell.Controls.Add(objRadio);
                    }
                }
                else
                {
                    objCell.Text = "<p class=\"text-warning\">問卷選項載入失敗。</p>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);
                return objTable;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Create_RelatedRadiobox", ex.Message);
                return null;
            }
        }


        /// <summary>
        /// QuestionType=14 or 15
        /// 產生下拉選單輸入控制項
        /// </summary>
        /// <param name="myRow"></param>
        /// <param name="MultiSelect">是否為多選題</param>
        /// <returns></returns>
        public Table Create_DropdownList(DataRow myRow, bool MultiSelect)
        {
            Table objTable = new Table();
            TableRow objRow;
            TableCell objCell;
            Label objLabel = new Label();
            DropDownList objControl = new DropDownList();
            Int64 QuestionID;
            string QTitle;
            string QDescription;
            string IsOption;

            try
            {
                QuestionID = Convert.ToInt64(myRow["QuestionID"]);
                QTitle = myRow["QTitle"].ToString();
                QDescription = myRow["QDescription"].ToString();
                IsOption = myRow["IsOptional"].ToString();

                objTable.ID = "fm-QTable-" + QuestionID.ToString();
                objTable.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                objTable.Width = Unit.Percentage(100);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.CssClass = "bg-info";
                objCell.Text = QTitle;
                if (QDescription != "")
                {
                    objCell.Text += "<br/><small>" + QDescription + "</small>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.Style.Add("padding-left", "30px");

                myData = Get_Option(QuestionID);
                if (myData != null)
                {
                    objControl.ID = "fm_QControl_" + QuestionID.ToString();
                    objControl.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    //if (IsOption == "N")
                    //{
                    //    objControl.CssClass = "easyui-combobox element-required";
                    //}
                    //else
                    //{
                    //objControl.CssClass = "easyui-combobox";
                    //}
                    objControl.DataSource = myData;
                    objControl.DataTextField = "SelectOption";
                    objControl.DataValueField = "SelectValue";
                    // 因採用jQuery Validate Plugin，無法使用easyui，所以不再支援多選下拉選單
                    //if (MultiSelect)
                    //{
                    //    objControl.Attributes.Add("multiple", "true");
                    //    objControl.Attributes.Add("multiline", "true");
                    //    objControl.Height = Unit.Pixel(50);
                    //}
                    //objControl.Attributes.Add("editable", "false");
                    //objControl.Width = Unit.Percentage(90);
                    objControl.DataBind();

                    if (myRow["ParentID"].ToString() != "0")
                    {
                        objControl.CssClass = "fmRelative_" + myRow["ParentID"].ToString();
                        objControl.Attributes.Add("disabled", "disabled");
                    }

                    objCell.Controls.Add(objControl);
                }
                else
                {
                    objCell.Text = "<p class=\"text-warning\">問卷選項載入失敗。</p>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);
                return objTable;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Create_Checkbox", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// QuestionType>100
        /// 產生系統預先定義的單選輸入控制項
        /// </summary>
        /// <param name="QuestionID"></param>
        /// <param name="QTitle"></param>
        /// <param name="QDescription"></param>
        /// <returns></returns>
        public Table Create_PreDefineInput(Int64 QuestionID, string QTitle, string QDescription, string TypeCode)
        {
            Table objTable = new Table();
            TableRow objRow;
            TableCell objCell;
            Label objLabel = new Label();
            RadioButtonList objRadio = new RadioButtonList();
            string strRepeatCount = "";

            try
            {
                objTable.ID = "fm-QTable-" + QuestionID.ToString();
                objTable.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                objTable.Width = Unit.Percentage(100);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.CssClass = "bg-info";
                objCell.Text = QTitle;
                if (QDescription != "")
                {
                    objCell.Text += "<br/><small>" + QDescription + "</small>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);

                objRow = new TableRow();
                objCell = new TableCell();
                objCell.Width = Unit.Percentage(100);
                objCell.Style.Add("padding-left", "30px");

                myData = Get_PreDefineOption(TypeCode);
                if (myData != null)
                {
                    objRadio.ID = "fm_QControl_" + QuestionID.ToString();
                    objRadio.CssClass = "option-item";
                    objRadio.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    objRadio.DataSource = myData;
                    objRadio.DataTextField = "SelectOption";
                    objRadio.DataValueField = "SelectValue";
                    //objRadio.RepeatDirection = RepeatDirection.Vertical;
                    objRadio.RepeatDirection = RepeatDirection.Horizontal ;
                    objRadio.RepeatLayout = RepeatLayout.Flow;
                    strRepeatCount = Get_RepeatItem_Count(QuestionID);
                    if (strRepeatCount == null)
                    {
                        objRadio.RepeatColumns = 5;
                    }
                    else
                    {
                        objRadio.RepeatColumns = Convert.ToInt32(strRepeatCount);
                    }
                    //objRadio.CssClass = "clear_table";
                    objRadio.DataBind();
                    objCell.Controls.Add(objRadio);
                }
                else
                {
                    objCell.Text = "<p class=\"text-warning\">問卷選項載入失敗。</p>";
                }
                objRow.Cells.Add(objCell);
                objTable.Rows.Add(objRow);
                return objTable;
            }
            catch (Exception ex)
            {
                ShareFunction.PutLog("Create_Radiobox", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 取得問題的選項資料
        /// </summary>
        /// <param name="QuestionID">問題流水編號</param>
        /// <returns>選項資料表</returns>
        private DataTable Get_Option(long QuestionID)
        {
            strSQL = "Select OptionID, ColumnNo, SelectOption, SelectValue, DefaultItem, FreeItem, SortID, JumpQID "
                    + "From OptionData "
                    + "Where QuestionID=@ID And Disabled='N' "
                    + "Order By ColumnNo, SortID";
            _DB.AddSqlParameter("@ID", QuestionID);
            myData = _DB.GetDataTable(strSQL);
            if (myData == null)
            {
                ShareFunction.PutLog("Get_Option", _DB.DBErrorMessage);
            }
            return myData;
        }

        private String Get_RepeatItem_Count(long QuestionID)
        {
            strSQL = "Select AttrValue From QuestionAttribute Where QuestionID=@ID And AttrName=N'RepeatColumns'";
            _DB.AddSqlParameter("@ID", QuestionID);
            return _DB.GetData(strSQL);
        }

        /// <summary>
        /// 取得影像圖片來源檔案位置
        /// </summary>
        /// <param name="QuestionID"></param>
        /// <returns></returns>
        private String Get_ImageSource(long QuestionID)
        {
            strSQL = "Select AttrValue From QuestionAttribute Where QuestionID=@ID And AttrName=N'ImageSource'";
            _DB.AddSqlParameter("@ID", QuestionID);
            return _DB.GetData(strSQL);
        }

        /// <summary>
        /// 取得預定義問題的選項資料
        /// </summary>
        /// <param name="TypeCode">預定義代碼</param>
        /// <returns>選項資料表</returns>
        private DataTable Get_PreDefineOption(string TypeCode)
        {
            strSQL = "Select SelectOption, SelectValue "
                    + "From DefaultOption "
                    + "Where Code=@Code And Disabled='N' "
                    + "Order By SortID";
            _DB.AddSqlParameter("@Code", TypeCode.ToUpper());
            myData = _DB.GetDataTable(strSQL);
            if (myData == null)
            {
                ShareFunction.PutLog("Get_PreDefineOption", _DB.DBErrorMessage);
            }
            return myData;
        }
    }
}