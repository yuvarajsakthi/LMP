import React from 'react';
import { Input, Form, Space, Button, Row, Col, Select } from 'antd';
import { ArrowLeftOutlined } from '@ant-design/icons';
import styles from './Addressinformation.module.css';
import { useLoanApplication } from '../../../../context/LoanApplicationContext';
import { NextButtonArrow } from '../../../../assets';
import { IndianStates, type AddressInformationDTO } from '../../../../types/loanApplicationCreate';

const { Option } = Select;

const indianStatesOptions = [
  { value: IndianStates.AndhraPradesh, label: 'Andhra Pradesh' },
  { value: IndianStates.ArunachalPradesh, label: 'Arunachal Pradesh' },
  { value: IndianStates.Assam, label: 'Assam' },
  { value: IndianStates.Bihar, label: 'Bihar' },
  { value: IndianStates.Chhattisgarh, label: 'Chhattisgarh' },
  { value: IndianStates.Goa, label: 'Goa' },
  { value: IndianStates.Gujarat, label: 'Gujarat' },
  { value: IndianStates.Haryana, label: 'Haryana' },
  { value: IndianStates.HimachalPradesh, label: 'Himachal Pradesh' },
  { value: IndianStates.Jharkhand, label: 'Jharkhand' },
  { value: IndianStates.Karnataka, label: 'Karnataka' },
  { value: IndianStates.Kerala, label: 'Kerala' },
  { value: IndianStates.MadhyaPradesh, label: 'Madhya Pradesh' },
  { value: IndianStates.Maharashtra, label: 'Maharashtra' },
  { value: IndianStates.Manipur, label: 'Manipur' },
  { value: IndianStates.Meghalaya, label: 'Meghalaya' },
  { value: IndianStates.Mizoram, label: 'Mizoram' },
  { value: IndianStates.Nagaland, label: 'Nagaland' },
  { value: IndianStates.Odisha, label: 'Odisha' },
  { value: IndianStates.Punjab, label: 'Punjab' },
  { value: IndianStates.Rajasthan, label: 'Rajasthan' },
  { value: IndianStates.Sikkim, label: 'Sikkim' },
  { value: IndianStates.TamilNadu, label: 'Tamil Nadu' },
  { value: IndianStates.Telangana, label: 'Telangana' },
  { value: IndianStates.Tripura, label: 'Tripura' },
  { value: IndianStates.UttarPradesh, label: 'Uttar Pradesh' },
  { value: IndianStates.Uttarakhand, label: 'Uttarakhand' },
  { value: IndianStates.WestBengal, label: 'West Bengal' },
  { value: IndianStates.Delhi, label: 'Delhi' },
  { value: IndianStates.Chandigarh, label: 'Chandigarh' },
  { value: IndianStates.Puducherry, label: 'Puducherry' }
];

interface AddressinformationProps {
  loanId?: string;
  onNext: () => void;
  onPrev: () => void;
  onNextStep?: () => void;
  onBackStep?: () => void;
}

const Addressinformation: React.FC<AddressinformationProps> = ({
  onNext,
  onPrev,
  onNextStep,
  onBackStep
}) => {
  const { state, dispatch } = useLoanApplication();
  const [form] = Form.useForm();

  React.useEffect(() => {
    if (state.formData.addressInformation) {
      form.setFieldsValue(state.formData.addressInformation);
    }
  }, [state.formData.addressInformation, form]);

  const handleSubmit = (values: Partial<AddressInformationDTO>) => {
    dispatch({
      type: 'UPDATE_FORM_DATA',
      payload: {
        section: 'addressInformation',
        data: values
      }
    });
    onNext();
    if (onNextStep) onNextStep();
  };

  const handleBack = () => {
    onPrev();
    if (onBackStep) onBackStep();
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h3 className={styles.title}>Address Information</h3>
        <p className={styles.subtitle}>Enter your address information as mentioned in IDs</p>
      </div>

      <Form 
        form={form}
        onFinish={handleSubmit} 
        layout="vertical"
        className={styles.form}
      >
        <Row gutter={16}>
          <Col span={24}>
            <Form.Item
              name="presentAddress"
              label="Present Address"
              rules={[{ required: true, message: 'Present address is required' }, { max: 250 }]}
            >
              <Input.TextArea rows={2} placeholder="Enter your present address" className={styles.input} />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={24}>
            <Form.Item
              name="permanentAddress"
              label="Permanent Address"
              rules={[{ required: true, message: 'Permanent address is required' }, { max: 250 }]}
            >
              <Input.TextArea rows={2} placeholder="Enter your permanent address" className={styles.input} />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={8}>
            <Form.Item name="district" label="District" rules={[{ required: true, message: 'District is required' }, { max: 100 }]}>
              <Input placeholder="Enter district" className={styles.input} />
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item name="state" label="State" rules={[{ required: true, message: 'State is required' }]}>
              <Select placeholder="Select state" className={styles.input}>
                {indianStatesOptions.map(state => (
                  <Option key={state.value} value={state.value}>
                    {state.label}
                  </Option>
                ))}
              </Select>
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item name="zipCode" label="Zip Code" rules={[{ required: true, message: 'Zip code is required' }, { max: 10 }]}>
              <Input placeholder="Enter zip code" className={styles.input} maxLength={10} />
            </Form.Item>
          </Col>
        </Row>

        <div className={styles.buttonContainer}>
          <Space size="large">
            <Button 
              onClick={handleBack}
              className={styles.backButton}
              icon={<ArrowLeftOutlined />}
            >
              BACK
            </Button>
            
            <Button
              type="primary"
              htmlType="submit"
              className={styles.nextButton}
            >
              NEXT
              <img 
                src={NextButtonArrow} 
                alt="Next" 
                className={styles.nextIcon}
              />
            </Button>
          </Space>
        </div>
      </Form>
    </div>
  );
};

export default Addressinformation;
