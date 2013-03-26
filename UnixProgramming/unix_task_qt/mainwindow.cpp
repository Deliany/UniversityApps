//
//  main.cpp
//  unix_task_by_silent
//
//  Created by Deliany Delirium on 12.03.13.
//  Copyright (c) 2013 Clear Sky. All rights reserved.
//
#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QStandardItemModel>
#include <QFileDialog>
#include <QMessageBox>
#include <QTextStream>
#include <fstream>
#include <sstream>
#include <iostream>
#include <vector>
#include <map>
using namespace std;

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    this->setFixedSize(this->geometry().width(),this->geometry().height());
    model = new QStandardItemModel();
    ui->listView->setModel( model );
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::on_toolButton_clicked()
{
    QString qs = QFileDialog::getOpenFileName(this, tr("Open File"), "", tr("Files (*.*)"));
    ui->lineEdit->setText(qs);
}

void MainWindow::on_pushButton_clicked()
{
    model->clear();
    QString filePath = ui->lineEdit->text();

    QFile file(filePath);
    if(!file.open(QIODevice::ReadOnly)) {
        QMessageBox::information(0, "Could not open file!", file.errorString());
        return;
    }

    QTextStream in(&file);
    vector<QString> words;

    while(!in.atEnd()) {
        QString line = in.readLine();
        QStringList fields = line.split(" ");
        foreach (QString text, fields)
            words.push_back(text);
    }
    file.close();

    // 1. convert all letters to upper case and replace spaces to new lines
    for_each(words.begin(), words.end(), MainWindow::convertWords());

    // 2. sort the result
    sort(words.begin(),words.end(), MainWindow::compareWords);

    // 3. count unique words
    map<QString, int> wordCount;
    for (vector<QString>::iterator it = words.begin(); it != words.end(); ++it) {
        ++wordCount[*it];
    }

    vector<pair<QString,int> > myVec(wordCount.begin(), wordCount.end());


    // 4. sort the result reversive
    sort(myVec.rbegin(), myVec.rend(), MainWindow::compareWordFrequency);

    // 5. print head 10 records
    int elem = 0;
    for (vector<pair<QString, int> >::iterator it = myVec.begin(); it != myVec.end(); ++it)
    {
        std::stringstream ss;
        ss << it->second;
        QString data = it->first +":"+QString::fromStdString(ss.str());

        QStandardItem *item;
        item = new QStandardItem();
        item->setData( data, Qt::DisplayRole );
        //item->setData( QImage(":/Pix/Pix.png"), Qt::DecorationRole );
        item->setEditable( false );
        model->appendRow( item );

        //cout << it->first <<" : "<< it->second << endl;
        ++elem;
        if (elem == 10) {
            break;
        }
    }
}

bool MainWindow::compareWords(QString w1, QString w2)
{
    return w1 < w2;
}

bool MainWindow::compareWordFrequency(pair<QString,int> w1, pair<QString,int> w2)
{
    return w1.second < w2.second;
}
