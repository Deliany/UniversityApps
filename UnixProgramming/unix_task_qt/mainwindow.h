#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QStandardItemModel>

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT
    
public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow();
    static bool compareWords(QString w1, QString w2);
    static bool compareWordFrequency(std::pair<QString,int> w1, std::pair<QString,int> w2);
    struct convertWord {
        void operator()(QChar& c)
        {
            if(c == ' ')
            {
                c = '\n';
            }
            else
            {
                c = c.toUpper();
            }
        }
    };

    struct convertWords {
        void operator()(QString& str)
        {
            std::for_each(str.begin(), str.end(), convertWord());
        }
    };
    
private slots:
    void on_pushButton_clicked();

    void on_toolButton_clicked();

private:
    Ui::MainWindow *ui;
    QStandardItemModel *model;
};

#endif // MAINWINDOW_H
