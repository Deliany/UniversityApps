import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;

/**
 * Created with IntelliJ IDEA.
 * User: Deliany
 * Date: 27.03.13
 * Time: 03:11
 * To change this template use File | Settings | File Templates.
 */
public class FileContentReaderManager {
    public static String ReadFile(String filePath) throws IOException {
        /*  Sets up a file reader to read the file passed on the command
       line one character at a time */
        FileReader input = new FileReader(filePath);

        /* Filter FileReader through a Buffered read to read a line at a
           time */
        BufferedReader bufRead = new BufferedReader(input);

        String line; 	// String that holds current file line
        int count = 0;	// Line number of count

        StringBuilder content = new StringBuilder();

        // Read through file one line at time. Print line # and line
        while ((line = bufRead.readLine()) != null) {

            content.append(line + '\n');
            //System.out.println(count+": "+line);
            count++;
        }
        bufRead.close();

        return content.toString();
    }
}
