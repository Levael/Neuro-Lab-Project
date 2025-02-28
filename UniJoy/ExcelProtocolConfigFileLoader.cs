﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

//For excel file reading.
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;       //microsoft Excel 14 object in references-> COM tab
using Params;

namespace UniJoy
{
    /// <summary>
    /// This class is used to read the data from excel protocol file and load it into the GuiInterface.
    /// The excel file may contain two sheets of parameters (for the trial) and of functions 
    /// (for the calling functions to analyze response, trajectory creation, etc).
    /// </summary>
    public class ExcelProtocolConfigFileLoader
    {
        /// <summary>
        /// Excel application handler.
        /// </summary>
        private Excel.Application _xlApp;

        /// <summary>
        /// Start the excel app to run.
        /// </summary>
        public ExcelProtocolConfigFileLoader()
        {
            _xlApp = new Excel.Application();
        }

        /// <summary>
        /// Closes the excel application and destroy it's running app.
        /// </summary>
        public void CloseExcelProtocolConfigFileLoader()
        {
            _xlApp.Quit();
        }

        /// <summary>
        /// Reads an excel protocol files and insert all the variables in the protocol with each variable attributes.
        /// </summary>
        /// <param name="protocolFilePath">The protocol file path to be read.</param>
        /// <param name="variables">The object where all the variables with their attributes will be saved.</param>
        public void ReadProtocolFile(string protocolFilePath, ref Variables variables)
        {
            Excel.Workbook xlWorkbook = _xlApp.Workbooks.Open(protocolFilePath);

            Excel._Worksheet xlWorksheet;
            try
            {
                xlWorksheet = xlWorkbook.Sheets["variables"];
            }
            catch (Exception)
            {
                MessageBox.Show("The sheets does not contain variables sheet or contain more than that specific sheets", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Excel.Range xlRange = xlWorksheet.UsedRange;

            object[,] valueArray = (object[,])xlRange.get_Value(
                Excel.XlRangeValueDataType.xlRangeValueDefault);

            string[,] excelStringValuesArray = Convert2DObjectsTo2DStrings(valueArray);

            //vars going to collect all the variables in the excel data with their attributes as dictionary of a 
            //variable name and all it's attributes in inner dictionary.
            variables._variablesDictionary = new Dictionary<string, Variable>();

            //collecting all the attributes for each variable (all variables have them).
            string[] attributes = new string[excelStringValuesArray.GetLength(1)];
            
            // attributes here are names of every column
            for (int i = 0; i < excelStringValuesArray.GetLength(1); i++)
            {
                attributes[i] = excelStringValuesArray[0, i];
            }

            //run along all the data lines.
            // k starts from 1 because first line is the attributes names
            for (int k = 1; k < excelStringValuesArray.GetLength(0); k++)
            {
                //making the new variable to be inserted into the protocol variables dictionary.
                Variable var = new Variable();
                var._description = new Dictionary<string, Param>();

                //run along the number of columns along the lines to collect the attributes of the specific variable.
                for (int i = 0; i < excelStringValuesArray.GetLength(1); i++)
                {
                    Param param = new Param();

                    param.MoogParameter = excelStringValuesArray[k, i];

                    if (excelStringValuesArray[k, i] != null)
                        param = DisassamblyDataAttributeValue(excelStringValuesArray[k, i]);

                    try
                    {
                        var._description.Add(attributes[i], param);
                    }
                    catch
                    {
                        MessageBox.Show("The attribute named " + attributes[i] + " is showing twice in the columns.",
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                try
                {
                    //adding the variable (line in the excel data file) into the dictionary of variables 
                    //with the variable name as the key.
                    variables._variablesDictionary.Add(var._description["name"].MoogParameter, var);
                }
                catch (Exception)
                {
                    MessageBox.Show("The sheet contain a parameter named " + var._description["name"].MoogParameter
                        + " showing twice.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            //clost the file.
            xlWorkbook.Close();
        }

        /// <summary>
        /// Writes a protocol file to the protocols folder.
        /// </summary>
        /// <param name="protocolFilePath">The protocol folder path with protocol name.</param>
        /// <param name="variables">The variables to write to the protocol.</param>
        /// <param name="checkboxesDictionary">A dictionary consists all gui checkboxes and their states.</param>
        public void WriteProtocolFile(string protocolFilePath, Variables variables, Dictionary<string, ButtonBase> checkboxesDictionary)
        {
            Excel.Workbook newProtocolFile = _xlApp.Workbooks.Add();

            Excel.Worksheet workSheet = newProtocolFile.Worksheets.Add();

            //change the sheet name to variables.
            workSheet.Name = "variables";

            //add the first line of heads titles to each column
            int columnIndex = 1;
            foreach (string title in variables._variablesDictionary.ElementAt(0).Value._description.Keys)
            {
                //add the title for each column of the excel file.
                workSheet.Cells[1, columnIndex] = title;
                //move next column title.
                columnIndex++;
            }

            //save each line according to the variable parameters.
            int rowIndex = 2;
            foreach (KeyValuePair<string, Variable> item in variables._variablesDictionary)
            {
                //reset the column index for the new line.
                columnIndex = 1;

                //if not a checkbox property
                if (checkboxesDictionary.Keys.Count(x => x == item.Key) == 0)
                {
                    foreach (string titleName in item.Value._description.Keys)
                    {
                        //write the column to the variable
                        workSheet.Cells[rowIndex, columnIndex] = item.Value._description[titleName].MoogParameter;
                        //go to next column for the same variable.
                        columnIndex++;
                    }
                }
                //if a checkbox property.
                else
                {
                    foreach (string titleName in item.Value._description.Keys)
                    {
                        //add the name of the variable checkbox.
                        if (titleName == "name")
                        {
                            workSheet.Cells[rowIndex, columnIndex] = item.Key;
                        }
                        else if (titleName == "parameters")
                        {
                            //write the column to the variable
                            if (checkboxesDictionary[item.Key] is RadioButton)
                                workSheet.Cells[rowIndex, columnIndex] =
                                    (checkboxesDictionary[item.Key] as RadioButton).Checked ? "1" : "0";
                            else if (checkboxesDictionary[item.Key] is CheckBox)
                                workSheet.Cells[rowIndex, columnIndex] =
                                    (checkboxesDictionary[item.Key] as CheckBox).Checked ? "1" : "0";
                            //todo:exception if the type is else.

                        }
                        else if (titleName == "status")
                        {
                            //write the column to the variable
                            workSheet.Cells[rowIndex, columnIndex] = "-1";
                        }
                        else
                        {
                            //write the column to the variable
                            workSheet.Cells[rowIndex, columnIndex] = "0";
                        }

                        //go to next column for the same variable.
                        columnIndex++;
                    }
                }
                //go to next line (for next variable)
                rowIndex++;
            }

            //add all checkboxes that are not in the excel file from the beginning.
            foreach (var item in checkboxesDictionary)
            {
                //reset the column index for the new line.
                columnIndex = 1;

                //if the checkbox not included in the variable dictionary.
                if (variables._variablesDictionary.Keys.Count(key => key == item.Key) == 0)
                {
                    foreach (string titleName in variables._variablesDictionary.ElementAt(0).Value._description.Keys)
                    {
                        //add the name of the variable checkbox.
                        if (titleName == "name")
                        {
                            workSheet.Cells[rowIndex, columnIndex] = item.Key;
                        }
                        else if (titleName == "parameters")
                        {
                            //write the column to the variable
                            if (item.Value is CheckBox)
                                workSheet.Cells[rowIndex, columnIndex] = (item.Value as CheckBox).Checked ? "1" : "0";
                            else if (item.Value is RadioButton)
                                workSheet.Cells[rowIndex, columnIndex] =
                                    (item.Value as RadioButton).Checked ? "1" : "0";
                            //todo:exception if the type is else.
                        }
                        else if (titleName == "status")
                        {
                            //write the column to the variable
                            workSheet.Cells[rowIndex, columnIndex] = "-1";
                        }
                        else
                        {
                            //write the column to the variable
                            workSheet.Cells[rowIndex, columnIndex] = "0";
                        }

                        //go to next column for the same variable.
                        columnIndex++;
                    }

                    //go to next line (for next variable)
                    rowIndex++;
                }
            }

            try//it is for the event when the file with the same name exists and the user checked the saving.
            {
                //save the file and close it.
                newProtocolFile.SaveAs(protocolFilePath + ".xlsx");
                newProtocolFile.Close();
            }
            catch { }
        }

        /// <summary>
        /// Converts a 2D array of object type to 2D array of string type.
        /// </summary>
        /// <param name="array">The 2D object array.</param>
        /// <returns>The 2d string array.</returns>
        private string[,] Convert2DObjectsTo2DStrings(object[,] array)
        {
            string[,] returnArray = new string[array.GetLength(0), array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    returnArray[i, j] = (array[i + 1, j + 1] == null) ? null : array[i + 1, j + 1].ToString();
                }
            }
            
            /*string[,] returnArray = new string[array.GetLength(0), array.GetLength(1)];
             // better option imho

            for (int i = 1; i <= array.GetLength(0); i++)
            {
                for (int j = 1; j <= array.GetLength(1); j++)
                {
                    returnArray[i, j] = (array[i, j] == null) ? null : array[i, j].ToString();
                }
            }*/

            return returnArray;
        }

        /// <summary>
        /// Dissasembly data attribute to its components (if it's a vector attribute for both the _MoogParameter and _landscapeParameters) for a Param class.
        /// </summary>
        /// <param name="attributeValue">The attribute value of the excel cell to be dissasembly.</param>
        /// <returns>The param disassemblied object acordding to the value.</returns>
        private Param DisassamblyDataAttributeValue(string attributeValue)
        {
            Param par = new Param();

            //split each vector of data for each robot to a list of components.
            par.MoogParameter = attributeValue;

            return par;
        }
    }
}
