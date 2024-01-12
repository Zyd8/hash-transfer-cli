# hash-transfer-cli
A **secure** and **lightweight** command-line tool for copying and moving files securely between locations. This tool supports a variety of hash algorithms to ensure data integrity during the transfer process. 
Additionally, it includes **error-correcting** measures to enhance the reliability of file transfers. Compatible with Linux and Windows.

### What is hashing?
Hashing in simple words takes an input such as a file and tries to return a unique output represented by a fixed string of characters. Let's say I create a  `my_file.txt`  file with its content being  `hello world` . If I feed it to a hash algorithm which is essentialy a mathematical formula like md5, it will output something like:  `6f5902ac237024bdd0c176cb93063dc4` . However, the moment I modify the content of that file to  `hello world.` , that very small change leads to an significantly different output:  `34521560c992eec462c9080dcf5b89e0` . Now, you might begin to see how big of a role hashing takes when it comes to data integrity.

### Why do file hashing during transfer operations? 
- To primarly avoid corruption in general. The probability of a corruption happening during a day-to-day file transfer is very unlikely. However, it is always not zero.
  - Handling of critical data. When it is absolutely necessary that a file must be identical to another like family pictures, because one single flip of a bit could render the image unreadable.
  - Handling of massive amounts of data. If you're copying Terabytes of data to another location, it's time-consuming to check each and every one of your ____ collection to see if it is not corrupted.
  - Assurance. It is reassuring to see for yourself that data integrity measures are being actively done. (Especially for "paranoids" like me).

## Installation
