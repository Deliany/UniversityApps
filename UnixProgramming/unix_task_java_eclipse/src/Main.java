import java.io.*;
import java.util.*;

public class Main {

    public static void main(String[] args) {
	// write your code here

        //String filePath = "/Users/Deliany/Desktop/UniversityApps/UnixProgramming/unix_task_by_silent_clang/unix_task_by_silent/frankenstein.txt";

        if (args.length != 1)
        {
            System.err.println("Usage: <filename>");
            System.exit(-1);
        }


        try
        {
            String fileContent = FileContentReaderManager.ReadFile(args[0]);
            Map<String,Integer> wordCount = UnixScripting.ProcessContent(fileContent);

            // 5. print head 10 records
            int elem = 0;
            for (Map.Entry<String, Integer> entry : wordCount.entrySet())
            {
                System.out.println(entry.getKey() + " : " + entry.getValue());
                ++elem;
                if (elem == 10)
                {
                    break;
                }
            }
        }
        // Catches any error conditions
        catch (IOException e)
        {
            System.err.println (e.getLocalizedMessage());
            System.exit(-1);
        }
    }


}
