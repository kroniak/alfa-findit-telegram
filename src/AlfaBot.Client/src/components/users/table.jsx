import React from "react";
import {withStyles} from '@material-ui/core/styles/';
import Table from '@material-ui/core/Table/';
import TableBody from '@material-ui/core/TableBody/';
import TableCell from '@material-ui/core/TableCell/';
import TableHead from '@material-ui/core/TableHead/';
import TableRow from '@material-ui/core/TableRow/';
import Paper from '@material-ui/core/Paper/';

const styles = theme => ({
    root: {
        width: '100%',
        marginTop: theme.spacing.unit * 3,
        overflowX: 'auto',
    },
    table: {
        minWidth: 700,
    },
});

const UserTable = props => {
    const {classes, users} = props;

    if (users.length > 0) {
        return (
            <Paper className={classes.root}>
                <Table className={classes.table}>
                    <TableHead>
                        <TableRow>
                            <TableCell>Telegram Name</TableCell>
                            <TableCell align="right">ChatId</TableCell>
                            <TableCell align="right">Name</TableCell>
                            <TableCell align="right">Phone</TableCell>
                            <TableCell align="right">EMail</TableCell>
                            <TableCell align="right">University</TableCell>
                            <TableCell align="right">Profession</TableCell>
                            <TableCell align="right">Course</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {users.map((user, i) => renderRow(user, i))}
                    </TableBody>
                </Table>
            </Paper>
        );
    } else return <div></div>
};

const renderRow = ({chatId, telegramName, name, phone, eMail, university, profession, course}, i) => {
    return (
        <TableRow key={i}>
            <TableCell component="th" scope="row">
                {telegramName}
            </TableCell>
            <TableCell align="right">{chatId === 0 ? "Не заполнено" : chatId}</TableCell>
            <TableCell align="right">{name}</TableCell>
            <TableCell align="right">{phone}</TableCell>
            <TableCell align="right">{eMail ? eMail : "Не заполнено"}</TableCell>
            <TableCell align="right">{university ? university : "Не заполнено"}</TableCell>
            <TableCell align="right">{profession ? profession : "Не заполнено"}</TableCell>
            <TableCell align="right">{course ? course : "Не заполнено"}</TableCell>
        </TableRow>
    )
};

export default withStyles(styles)(UserTable);