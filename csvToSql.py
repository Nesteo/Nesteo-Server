import mysql.connector
import re
from enum import Enum

# in order to run this script successfully you must do a few things first:

# 1 install mysql
# 2 open this script and type in your information in line 128
# 3 convert the file to a csv WARNING some converters will convert special chacters into different characters then others, I used xlsx2csv.  
#   On a debian linux distro you can install this by typing 'sudo apt get install xlsx2csv'
#   run this like 'xlsx2csv filename.xlsx > filename.csv' 
# 4 Type in the file name and file path at the bottom and you should be good to go !!!!  

#connect with your information
class QType(Enum):
    INT=1
    DOUBLE=2
    VARCHAR=3

class Queries:
    #qType=None
    #columnName =""
    #row =[]
     def __init__(self, qType, columnName, row):
        self.qType=qType
        self.columnName=columnName
        self.row=row
        

def convertTo2DArray(filePath):
    query=[[]]
    with open(filePath) as fp:
        j=0
        for cnt, line in enumerate(fp):
            i=0
            query.append([])
            for c in line.split(","): 
                query[j].append([])
                c=c.split('\n')[0]
                c=c.split('\xf6gel')[0]
                if c=='':
                    c="NULL"
                query[j][i].append(c)
                i+=1
            j+=1
    return query
# gets the type
def convertToSqlObj(query):
    total=[]
    for i in range(0, len(query[0])):
        string =""
        qType=""
        row=[]
        queries=None
        for j in range(1, len(query)-1):
            string =string+str(query[j][i][0])
            row.append(query[j][i])
        #Type Integer
        if bool(re.search("^[0-9]*$",string))and string != "":
            qtype ="INT"
        #Type double
        elif bool(re.search("^(\d|\.(\d*\.)*\d)*$",string)) and string != "":
            qtype= "FLOAT(53)"
        #Type string
        else:
            qtype="VARCHAR(255)"
        total.append(Queries(qtype,query[0][i],row))
    return total
        



def createTable(table, tableName):
    insert ="CREATE TABLE "+tableName+" ( "
    for cnt, i in enumerate(table):
        insert=insert+str(i.columnName[0].replace(" ","_").replace("-","_"))+" "+str(i.qType)
        if cnt<len(table)-1:
            insert+=" , "
    insert+=" );"
    return insert
    
def insertInto(table, tableName):
    insert="INSERT INTO "+tableName+"( "
    for cnt, x in enumerate(table):
        insert+= " "+str(x.columnName[0].replace(" ","_").replace("-","_"))
        if cnt<len(table)-1:
            insert+=" , "
    insert+=" ) "
    return insert

def insertValues(table, insertMe ):
    ins=[]
    #insert =insertMe
    insert=insertMe +" VALUES ( "
    for x, row in enumerate(table):
        if x>0:    
            for y, col in enumerate(row):
                if str(row[y][0])=="NULL":
                    insert=insert+" "+str(row[y][0])+" "
                else:
                    insert=insert+" '"+str(row[y][0])+"' "
                
                if y<len(table[0])-1:
                    insert+=" , "
            if x<len(table)-1:
                insert +=" ); "
                ins.append(insert)
            if x<len(table)-2:
                insert=insertMe
                insert+=" VALUES ( "
    return ins
            
        
    
# enter the file path

# option 0 create table and insert data
# option 1 just create table
# option 2 just insert data

def excuteSql(filePath, fileName, option):
    filePath ="Kontrolldaten-2012.csv"
    tableName="Kontrolldaten"
    tableArr=convertTo2DArray(filePath)
    table=convertToSqlObj(tableArr)
    # enter the file name
    makeTables=createTable(table, tableName)
    insertMe=insertInto(table, tableName)

    insertVals=insertValues(tableArr,insertMe)

    connection = mysql.connector.connect(
        host="localhost",
        user="root",
        passwd="root",
        database="nesto"
    )

    mycursor = connection.cursor(buffered=True)

    if option == 0 or option == 1:
        mycursor.execute(makeTables)
    
    if option == 0 or option == 2:    
        for x in insertVals:
            mycursor.execute(x)
            connection.commit()



#################################################################################
# main functions call one of these functions 

def insertData(filePath, fileName):
    excuteSql(filePath,fileName,2)

def createTable(filePath, fileName):
    excuteSql(filePath,fileName,1)

def createTableAndInsertData(filePath, fileName):
    excuteSql(filePath,fileName,0)

################################################################################

#createTableAndInsertData("Kontrolldaten-2012.csv","Kontrolldaten")

#insertData("Kontrolldaten-2012.csv","Kontrolldaten")


