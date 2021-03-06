﻿/******************************************************************************/
/****             Generated by IBExpert 1/29/2015 11:37:35 AM              ****/
/******************************************************************************/

/******************************************************************************/
/****     Following SET SQL DIALECT is just for the Database Comparer      ****/
/******************************************************************************/

/******************************************************************************/
/****                                Tables                                ****/
/******************************************************************************/


CREATE GENERATOR GEN_MEMO_ID;

CREATE TABLE MEMO (
    ID           INTEGER NOT NULL,
    MEMO         BLOB SUB_TYPE 1 SEGMENT SIZE 100 CHARACTER SET ASCII   -- BLOB SUB_TYPE 1 SEGMENT SIZE 100 CHARACTER SET ASCII  OR VARCHAR(256)
);




/******************************************************************************/
/****                               Indices                                ****/
/******************************************************************************/

CREATE INDEX MEMO_IDX1 ON MEMO COMPUTED BY (UPPER(TRIM(CAST(SUBSTRING(memo from 1 for 128) as VARCHAR(128)))));


/******************************************************************************/
/****                               Triggers                               ****/
/******************************************************************************/


SET TERM ^ ;



/******************************************************************************/
/****                         Triggers for tables                          ****/
/******************************************************************************/



/* Trigger: MEMO_BI */
CREATE OR ALTER TRIGGER MEMO_BI FOR MEMO
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (new.id is null) then
    new.id = gen_id(gen_memo_id,1);
end
^


SET TERM ; ^



/******************************************************************************/
/****                              Privileges                              ****/
/******************************************************************************/


/* Privileges of users */
GRANT SELECT, REFERENCES ON MEMO TO GONE;