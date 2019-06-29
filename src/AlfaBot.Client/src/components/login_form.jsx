import React, {Component} from "react";
import Plate from "arui-feather/plate";
import Heading from "arui-feather/heading";
import Input from "arui-feather/input";
import FormField from "arui-feather/form-field";
import Form from "arui-feather/form";
import Button from "arui-feather/button";
import Notification from "arui-feather/notification";

class LoginForm extends Component {
    constructor(props) {
        super(props);

        this.state = {
            userField: "admin",
            passwordField: "12345678"
        };

        this.onSubmitForm = this.onSubmitForm.bind(this);
    }

    onChangeUserInputValue = value => {
        this.setState({
            userField: value
        });
    };

    onChangePasswordInputValue = value => {
        this.setState({
            passwordField: value
        });
    };

    /**
     * Отправка формы
     * @param {Event} event событие отправки формы
     */
    onSubmitForm() {
        const {userField, passwordField} = this.state;
        if (userField && passwordField) {
            this.props.logUser(userField, passwordField);

            this.setState({
                    passwordField: "",
                    notifyVisible: false
                }
            );
        }
    };

    componentWillReceiveProps(nextProps, nextContext) {
        if (nextProps.authError) this.setState({
            notifyVisible: true
        })
    }

    render() {
        const {userField, passwordField} = this.state;
        return (
            <div style={{"marginBottom": "10px"}}>
                <Plate hasCloser={false}>
                    <Heading size='m'>
                        Пожалуйста войдите
                    </Heading>
                    <div>
                        <Notification
                            visible={this.state.notifyVisible}
                            status='error'
                            offset={100}
                            stickTo='right'
                            title='Ошибка входа'
                            onCloseTimeout={() => {
                                this.setState({notifyVisible: false});
                            }}
                            onCloserClick={() => {
                                this.setState({notifyVisible: false});
                            }}
                        >
                            Что то произошло не так! Скорее всего логин и пароль неверный
                        </Notification>
                        <Form onSubmit={this.onSubmitForm}>
                            <FormField size='m'>
                                <Input
                                    size='m'
                                    placeholder='Логин'
                                    type='text'
                                    value={userField}
                                    onChange={this.onChangeUserInputValue}
                                />
                            </FormField>
                            <FormField size='m'>
                                <Input
                                    size='m'
                                    placeholder='Пароль'
                                    type='password'
                                    value={passwordField}
                                    onChange={this.onChangePasswordInputValue}
                                />
                            </FormField>
                            <FormField>
                                <Button view='extra' type='submit'>Войти</Button>
                            </FormField>
                        </Form>
                    </div>
                </Plate>
            </div>)
    }
}

export default LoginForm;