import org.junit.Test;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.PrintWriter;
import java.io.UnsupportedEncodingException;

import static junit.framework.Assert.assertEquals;

/**
 * Created with IntelliJ IDEA.
 * User: Deliany
 * Date: 27.03.13
 * Time: 03:15
 * To change this template use File | Settings | File Templates.
 */
public class FileContentReaderManagerTest {
    @Test
    public void testReadFile() throws Exception {
        String fileName = "file.txt";
        String testContent = "1\n2\n3\n";
        createFileAndWriteText(fileName, testContent);

        String content = FileContentReaderManager.ReadFile(fileName);
        assertEquals("File natural content must be same as read content", content, testContent);
    }

    @Test (expected = FileNotFoundException.class)
    public void testReadFile2() throws Exception {
        String fileName = "non-existent-file.txt";
        String content = FileContentReaderManager.ReadFile(fileName);
    }

    public void createFileAndWriteText(String fileName, String text) throws FileNotFoundException, UnsupportedEncodingException {
        PrintWriter writer = new PrintWriter(fileName, "UTF-8");
        writer.print(text);
        writer.close();
    }
}
