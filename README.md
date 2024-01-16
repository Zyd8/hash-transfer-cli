# hash-transfer-cli
A **secure** and **lightweight** command-line tool for copying and moving files securely between locations. This tool supports a variety of hash algorithms to ensure data integrity during the transfer process. 
Additionally, it includes **error-correcting** measures to enhance the reliability of file transfers. It is compatible with both Linux and Windows.

## What is hashing?
Hashing in simple words takes an input such as a file, then tries to return a unique output represented by a fixed string of characters. Let's say I create a  `my_file.txt`  file with its content being  `hello world` . If I feed it to a hash algorithm which is essentialy a mathematical formula like md5, it will output something like:  `6f5902ac237024bdd0c176cb93063dc4` . However, the moment I modify the content of that file to  `hello world.` , that very small change leads to a significantly different output:  `34521560c992eec462c9080dcf5b89e0` . Now, you might begin to see how big of a role hashing takes when it comes to data integrity.

## Why do file hashing during transfer operations? 
- To primarly avoid corruption in general. The probability of a corruption happening during a day-to-day file transfer is very unlikely. However, it is always not zero.
  - Handling of critical data. When it is absolutely necessary that a file must be identical to another like family pictures because one single flip of a bit could render the image unreadable.
  - Handling of massive amounts of data. If you're copying Terabytes of data to another location, it is very time-consuming to check each and every one of your ____ collection to see if it is not corrupted.
  - Assurance. It is reassuring to see for yourself that data integrity measures are being actively done. (Especially for "paranoids" like me).

# Installation (Windows and Linux)

## GUI installation
In the [Release](https://github.com/Zyd8/hash-transfer-cli) section, download the correcy package according to your operating system.

## CLI installation
Open the terminal or cmd and enter: `wget [link]`.

# Usage

> `< Required >` ` [ Optional ] `

```
<hash-transfer-cli> <source> <destination> [-m/--mode] [-h/--hash]
```
- hash-transfer-cli - the entry point of the application.
- source - the file or directory you want to transfer.
- destination - the directory you want the source to be inside of.
- mode - the transfer modes.
  - copy (default) - copies the file or directory from source to destination.
  - cut - does the same thing as copy but the source file or directory is deleted after the transfer operation
- hash - the hash algorithms (sorted starting from the least secure to the most secure; starting from the fastest to lowest).
  - crc32
  - crc64
  - md5 (default)
  - sha1
  - sha256
  - sha512

## For windows

Launch cmd and `cd` your way into the repository folder where hash-transfer-cli.exe is located.

Run the program via:
  
```
hash-transfer-cli absolute:\path\to\source absolute:\path\to\destination
```

> Example: `hash-transfer-cli C:\Users\windows10\Downloads\my_file_or_directory C:\Users\windows10\Documents -h crc32 -m copy`

Or add the repo directory to PATH so that you can call the executable without being in the repository directory.

```
hash-transfer-cli absolute:\path\to\source absolute:\path\to\destination
```

## For Linux

Launch the terminal and `cd` your way into the repository folder where hash-transfer-cli is located.

```
./hash-transfer-cli /absolute/path/to/source /absolute/path/to/destination
```

> Example: `./hash-transfer-cli /home/user/Downloads/my_file_or_directory /home/user/Documents --hash sha512`

Or, again, add the repo directory to PATH so that you can call the executable without being in the repository directory.

> If the terminal outputs `Permission Denied`, then you must enable "Executable as Program" in the properties of hash-transfer-cli.














